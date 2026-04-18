using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Mappings;
using Modules.Tasks.Views.Services;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls.TaskItemView;

[AddINotifyPropertyChangedInterface]
public class TaskItemViewModel : BaseViewModel, ITaskItemViewModel
{
    private readonly IMediator _mediator;
    private readonly OneEditorOpenService _oneEditorOpenService;
    private string _contentRollback = string.Empty;
    private bool _isDone;

    public TaskItemViewModel(IMediator mediator,
        OneEditorOpenService oneEditorOpenService,
        IEventAggregator eventAggregator,
        IAppSettings appSettings,
        IOverlayPageNavigationService overlayPageNavigationService,
        string content)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(appSettings);
        ArgumentNullException.ThrowIfNull(overlayPageNavigationService);

        _mediator = mediator;
        _oneEditorOpenService = oneEditorOpenService;

        Content = new DynamicTextBoxViewModel(
            focusOnEditMode: true,
            enterActionOnLostFocus: appSettings.TaskPageSettings.ExitEditOnFocusLost,
            toolbarCloseOnLostFocus: false,
            acceptsTab: true);
        
        Content.EnterAction = ExitEditItem;
        Content.SetContent(content);

        Cmd = new TaskItemCommandsViewModel(this, mediator, eventAggregator, overlayPageNavigationService);

        EnableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = true);
        DisableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = false);

        // CheckBox and Combobox changes the viewmodel properties directly, only need to persist the changes
        ColorChangedNotification = new NotifiableObject(UpdateTask);
    }

    public int Id { get; set; }
    public required int CategoryId { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string MarkerColor { get; set; }
    public string BorderColor { get; set; }
    public string BackgroundColor { get; set; }

    public DynamicTextBoxViewModel Content { get; }
    public TaskItemCommandsViewModel Cmd { get; }

    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;
            Content.TextOpacity = IsDone ? 0.3 : 1.0;
            Opacity = IsDone ? 0.5 : 1.0;
        }
    }

    // Used for checkbox opacity
    public double Opacity { get; private set; } = 1.0;

    public bool IsAnyReminderOn { get; set; }
    public bool Pinned { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted => DeletedDate.HasValue;

    public bool IsQuickActionsEnabled { get; set; }

    public bool DetailsVisible { get; set; }

    public List<TaskItemVersionViewModel> Versions { get; set; } = [];

    public ObservableCollection<TagItemOnTaskViewModel> Tags { get; set; }
    public int VersionCount => Versions.Count;
    public bool HasHistory => VersionCount != 0;
    public bool IsFirstItem { get; set; }

    // Commands
    public ICommand EnableQuickActionsCommand { get; }
    public ICommand DisableQuickActionsCommand { get; }
    public INotifiableObject ColorChangedNotification { get; }

    void ITaskItemViewModel.EditItem()
    {
        // Save the content before editing for a possible rollback
        _contentRollback = Content.GetContent();

        // Enable editing
        Content.IsEditMode = true;
        _oneEditorOpenService.EditMode(this);
    }

    public async void ExitEditItem()
    {
        try
        {
            if (Content.IsEmpty)
            {
                // Empty content is rejected, roll back the previous content.
                Content.SetContent(_contentRollback);
            }
            else if (Content.GetContent() != _contentRollback)
            {
                ModificationDate = DateTime.Now;
                UpdateTask();

                var versionList = await _mediator.Send(new TaskItemVersionsQuery { TaskId = Id });
                Versions = versionList.MapToViewModelList(_mediator);

                OnPropertyChanged(nameof(Versions));
                OnPropertyChanged(nameof(VersionCount));
            }

            Content.IsEditMode = false;
            Content.IsToolbarOpen = false;
            _oneEditorOpenService.DisplayMode(this);
        }
        catch (Exception ex)
        {
            Content.IsEditMode = false;
            Content.IsToolbarOpen = false;
            _oneEditorOpenService.DisplayMode(this);
            System.Diagnostics.Trace.TraceError($"{nameof(ExitEditItem)} failed: {ex}");
        }
    }

    async void ITaskItemViewModel.UpdateHistory()
    {
        try
        {
            var versionList = await _mediator.Send(new TaskItemVersionsQuery { TaskId = Id });
            Versions = versionList.MapToViewModelList(_mediator);

            OnPropertyChanged(nameof(Versions));
            OnPropertyChanged(nameof(VersionCount));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"UpdateHistory failed: {ex}");
        }
    }

    void ITaskItemViewModel.UpdateTask() => UpdateTask();

    private void UpdateTask() => _mediator.Send(new UpdateTaskCommand { Task = this.Map() });
}
