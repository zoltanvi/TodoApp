using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Mappings;
using Modules.Tasks.Views.Services;
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

    public TaskItemViewModel(IMediator mediator, OneEditorOpenService oneEditorOpenService)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);

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
        DeleteItemCommand = new RelayCommand(DeleteItem);

        ToggleIsDoneCommand = new RelayCommand(ToggleIsDone);
        PinItemCommand = new RelayCommand(PinItem);
        UnpinItemCommand = new RelayCommand(UnpinItem);
        IsDoneModifiedCommand = new RelayCommand(UpdateTaskIsDone);

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
        }

        TextEditorViewModel.IsEditMode = false;
        TextEditorViewModel.IsToolbarOpen = false;
        _oneEditorOpenService.DisplayMode(this);
    }

    private void DeleteItem() => DeleteTaskItemRequestedEvent.Invoke(new DeleteTaskItemRequestedEvent { TaskId = Id });

    private void ToggleIsDone()
    {
        IsDone ^= true;
        UpdateTask();
    }

    private void PinItem() => PinTaskItemRequestedEvent.Invoke(new PinTaskItemRequestedEvent { TaskId = Id });
    
    private void UnpinItem() => UnpinTaskItemRequestedEvent.Invoke(new UnpinTaskItemRequestedEvent { TaskId = Id });

    private void UpdateTaskIsDone()
    {
        if (IsDone)
        {
            FinishTaskItemRequestedEvent.Invoke(new FinishTaskItemRequestedEvent { TaskId = Id });
        }
        else
        {
            UnfinishTaskItemRequestedEvent.Invoke(new UnfinishTaskItemRequestedEvent{TaskId = Id});
        }
    }

    private void UpdateTask() => _mediator.Send(new UpdateTaskCommand { Task = this.Map() });
}
