using MediatR;
using Modules.Common.DataBinding;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Mappings;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

public class TaskItemViewModel
{
    private readonly IMediator _mediator;
    private string _contentRollback = string.Empty;
    private bool _isDone;

    public TaskItemViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;

        // TODO: make it dynamic somehow
        TextEditorViewModel = new RichTextEditorViewModel(
            focusOnEditMode: true,
            enterActionOnLostFocus: AppSettings.Instance.TaskPageSettings.ExitEditOnFocusLost,
            toolbarCloseOnLostFocus: false,
            acceptsTab: true);

        EnableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = true);
        DisableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = false);
        EditItemCommand = new RelayCommand(EditItem);
        
        
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

    public string ContentPreview => TextEditorViewModel.DocumentContentPreview;

    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;
            TextEditorViewModel.TextOpacity = IsDone ? 0.25 : 1.0;
        }
    }

    // IsReminderOn AND TextEditorViewModel.IsDisplayMode
    public bool IsAnyReminderOn { get; set; }

    // Pinned AND TextEditorViewModel.IsDisplayMode
    public bool Pinned { get; set; }

    // AppSettings.TaskQuickActionSettings.AnyEnabled AND TextEditorViewModel.IsDisplayMode
    public bool IsHiddenButtonPanelVisible { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsQuickActionsEnabled { get; set; }

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

    private void EditItem()
    {
        // Save the content before editing for a possible rollback
        _contentRollback = Content;

        // Enable editing
        TextEditorViewModel.IsEditMode = true;
        //IoC.OneEditorOpenService.EditMode(this);
    }

    private void ToggleIsDone()
    {
        IsDone ^= true;
        UpdateTask();
    }
    private void PinItem()
    {
        _mediator.Publish(new PinTaskItemRequestedEvent{ TaskId = Id });
    }

    private void UnpinItem()
    {
        _mediator.Publish(new UnpinTaskItemRequestedEvent { TaskId = Id });
    }

    private void UpdateTaskIsDone()
    {
        if (IsDone)
        {
            _mediator.Publish(new FinishTaskItemRequestedEvent { TaskId = Id });
        }
        else
        {
            _mediator.Publish(new UnfinishTaskItemRequestedEvent { TaskId = Id });
        }
    }

    private void UpdateTask()
    {
        _mediator.Send(new UpdateTaskCommand { Task = this.Map() });
    }
}
