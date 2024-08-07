using MediatR;
using Modules.Common.DataBinding;
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

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemViewModel : BaseViewModel, ITaskItemViewModel
{
    private readonly IMediator _mediator;
    private readonly OneEditorOpenService _oneEditorOpenService;
    private string _contentRollback = string.Empty;
    private bool _isDone;

    public TaskItemViewModel(
        IMediator mediator, 
        OneEditorOpenService oneEditorOpenService,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _mediator = mediator;
        _oneEditorOpenService = oneEditorOpenService;

        // TODO: make it dynamic somehow
        TextEditorViewModel = new RichTextEditorViewModel(
            focusOnEditMode: true,
            enterActionOnLostFocus: AppSettings.Instance.TaskPageSettings.ExitEditOnFocusLost,
            toolbarCloseOnLostFocus: false,
            acceptsTab: true);
        TextEditorViewModel.EnterAction = ExitEditItem;

        Cmd = new TaskItemCommandsViewModel(this, mediator, eventAggregator);

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

    public RichTextEditorViewModel TextEditorViewModel { get; }

    public TaskItemCommandsViewModel Cmd { get; }

    public string Content
    {
        get => TextEditorViewModel.DocumentContent;
        set => TextEditorViewModel.DocumentContent = value;
    }

    public string ContentPreview
    {
        get => TextEditorViewModel.DocumentContentPreview;
        set => TextEditorViewModel.DocumentContentPreview = value;
    }

    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;
            TextEditorViewModel.TextOpacity = IsDone ? 0.3 : 1.0;
            Opacity = IsDone ? 0.5 : 1.0;
        }
    }

    // Used for checkbox opacity
    public double Opacity { get; private set; } = 1.0;

    public bool IsAnyReminderOn { get; set; }
    public bool Pinned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsQuickActionsEnabled { get; set; }

    public bool DetailsVisible { get; set; }

    public List<TaskItemVersionViewModel> Versions { get; set; }

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
        _contentRollback = Content;

        // Enable editing
        TextEditorViewModel.IsEditMode = true;
        _oneEditorOpenService.EditMode(this);
    }

    public void ExitEditItem()
    {
        if (TextEditorViewModel.IsContentEmpty)
        {
            // Empty content is rejected, roll back the previous content.
            Content = _contentRollback;
        }
        else if (Content != _contentRollback)
        {
            //Modifications are accepted, update task
            ModificationDate = DateTime.Now;
            UpdateTask();

            var versionList = _mediator.Send(new TaskItemVersionsQuery { TaskId = Id }).Result;
            Versions = versionList.MapToViewModelList(_mediator);

            OnPropertyChanged(nameof(Versions));
            OnPropertyChanged(nameof(VersionCount));
        }

        TextEditorViewModel.IsEditMode = false;
        TextEditorViewModel.IsToolbarOpen = false;
        _oneEditorOpenService.DisplayMode(this);
    }

    void ITaskItemViewModel.UpdateTask() => UpdateTask();

    private void UpdateTask() => _mediator.Send(new UpdateTaskCommand { Task = this.Map() });
}
