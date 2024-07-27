using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Views.Events;
using Prism.Events;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemCommandsViewModel : BaseViewModel
{
    private readonly TaskItemViewModel _taskItem;
    private readonly IMediator _mediator;
    private readonly ITaskItemViewModel _taskItemInternal;
    private readonly IEventAggregator _eventAggregator;

    public TaskItemCommandsViewModel(
        TaskItemViewModel taskItem,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItem);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItem = taskItem;
        _mediator = mediator;
        _taskItemInternal = taskItem;
        _eventAggregator = eventAggregator;

        IsDoneModifiedCommand = new RelayCommand(() =>
        {
            if (_taskItem.IsDone)
            {
                _eventAggregator.GetEvent<TaskItemCheckedEvent>().Publish(_taskItem.Id);
            }
            else
            {
                _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Publish(_taskItem.Id);
            }
        });

        EditItemCommand = new RelayCommand(_taskItemInternal.EditItem);
        ToggleIsDoneCommand = new RelayCommand(() =>
        {
            _taskItem.IsDone ^= true;
            _taskItemInternal.UpdateTask();
        });

        PinItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemPinClickedEvent>().Publish(_taskItem.Id));
        UnpinItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Publish(_taskItem.Id));
        DeleteItemCommand = new RelayCommand(() => eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Publish(_taskItem.Id));
        ToggleDetailsCommand = new RelayCommand(() => _taskItem.DetailsVisible ^= true);
        ShowTagSelectorCommand = new RelayCommand(() => _mediator.Send(new OpenTagSelectorCommand { TaskId = _taskItem. Id }));



    }

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

    public ICommand SortByStateCommand { get; }
    public ICommand SplitLinesCommand { get; }
    public ICommand SortByCreationDateCommand { get; }
    public ICommand SortByCreationDateDescCommand { get; }
    public ICommand SortByModificationDateCommand { get; }
    public ICommand SortByModificationDateDescCommand { get; }
    public ICommand MoveToTopCommand { get; }
    public ICommand MoveToBottomCommand { get; }
    public ICommand MoveToCategoryCommand { get; }

    //InactiveCategories
    
    public ICommand MoveAllToCategoryCommand { get; }
    public ICommand MoveAllCompletedToCategoryCommand { get; }
    public ICommand MoveAllIncompleteToCategoryCommand { get; }
    public ICommand ResetAllStatesCommand { get; }
    public ICommand ResetAllColorsCommand { get; }
    public ICommand ResetColorsCommand { get; }
    public ICommand ResetBackgroundColorsCommand { get; }
    public ICommand ResetBorderColorsCommand { get; }
    public ICommand DeleteAllCommand { get; }
    public ICommand DeleteCompletedCommand { get; }
    public ICommand DeleteIncompleteCommand { get; }
    //
    //
}
