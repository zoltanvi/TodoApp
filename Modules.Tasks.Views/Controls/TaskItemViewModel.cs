using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Mappings;
using Modules.Tasks.Views.Services;
using Prism.Events;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemViewModel : BaseViewModel
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

        ToggleDetailsCommand = new RelayCommand(() => DetailsVisible ^= true);
        EnableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = true);
        DisableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = false);
        EditItemCommand = new RelayCommand(EditItem);
        DeleteItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Publish(Id));

        ToggleIsDoneCommand = new RelayCommand(() =>
        {
            IsDone ^= true;
            UpdateTask();
        });

        PinItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemPinClickedEvent>().Publish(Id));
        UnpinItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Publish(Id));
        IsDoneModifiedCommand = new RelayCommand(() =>
        {
            if (IsDone)
            {
                eventAggregator.GetEvent<TaskItemCheckedEvent>().Publish(Id);
            }
            else
            {
                eventAggregator.GetEvent<TaskItemUncheckedEvent>().Publish(Id);
            }
        });

        ShowTagSelectorCommand = new RelayCommand(() => _mediator.Send(new OpenTagSelectorCommand { TaskId = Id }));

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
            TextEditorViewModel.TextOpacity = IsDone ? 0.25 : 1.0;
        }
    }

    public bool IsAnyReminderOn { get; set; }
    public bool Pinned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsQuickActionsEnabled { get; set; }

    public bool DetailsVisible { get; set; }

    public List<TaskItemVersionViewModel> Versions { get; set; }

    public TagItemOnTaskViewModel? Tag { get; set; }
    public bool HasAnyTags => Tag != null;

    public int VersionCount => Versions.Count;

    // Commands
    public ICommand IsDoneModifiedCommand { get; }
    public ICommand EditItemCommand { get; }
    public ICommand ToggleIsDoneCommand { get; }
    public ICommand OpenReminderCommand { get; }
    public ICommand PinItemCommand { get; }
    public ICommand UnpinItemCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public ICommand EnableQuickActionsCommand { get; }
    public ICommand DisableQuickActionsCommand { get; }

    public INotifiableObject ColorChangedNotification { get; }
    public ICommand ToggleDetailsCommand { get; }
    public ICommand ShowTagSelectorCommand { get; }


    private void EditItem()
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
            Versions = versionList.MapToViewModelList();

            OnPropertyChanged(nameof(Versions));
            OnPropertyChanged(nameof(VersionCount));
        }

        TextEditorViewModel.IsEditMode = false;
        TextEditorViewModel.IsToolbarOpen = false;
        _oneEditorOpenService.DisplayMode(this);
    }

    private void UpdateTask() => _mediator.Send(new UpdateTaskCommand { Task = this.Map() });
}
