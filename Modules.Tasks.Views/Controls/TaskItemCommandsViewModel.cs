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

        PinItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Publish(_taskItem.Id));
        UnpinItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Publish(_taskItem.Id));
        DeleteItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Publish(_taskItem.Id));
        ToggleDetailsCommand = new RelayCommand(() => _taskItem.DetailsVisible ^= true);
        ShowTagSelectorCommand = new RelayCommand(() => _mediator.Send(new OpenTagSelectorCommand { TaskId = _taskItem. Id }));

        SortByStateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.State);
        SortByCreationDateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.CreationDate, true);
        SortByCreationDateDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.CreationDate);
        SortByModificationDateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.ModificationDate, true);
        SortByModificationDateDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.ModificationDate);
        SortByContentCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.Content, true);
        SortByContentDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.Content);
    }
    
    public ICommand IsDoneModifiedCommand { get; }
    public ICommand EditItemCommand { get; }
    public ICommand ToggleIsDoneCommand { get; }
    public ICommand OpenReminderCommand { get; }
    public ICommand PinItemCommand { get; }
    public ICommand UnpinItemCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public ICommand ToggleDetailsCommand { get; }
    public ICommand ShowTagSelectorCommand { get; }

    public ICommand SplitLinesCommand { get; }

    public ICommand SortByStateCommand { get; }
    public ICommand SortByCreationDateCommand { get; }
    public ICommand SortByCreationDateDescCommand { get; }
    public ICommand SortByModificationDateCommand { get; }
    public ICommand SortByModificationDateDescCommand { get; }
    public ICommand SortByContentCommand { get; }
    public ICommand SortByContentDescCommand { get; }

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

    private ICommand CreateSortCommand(TaskSortingRequestedPayload.SortByProperty sortBy, bool ascending = false)
    {
        return new RelayCommand(() =>
            _eventAggregator.GetEvent<TaskSortingRequestedEvent>().Publish(
                new TaskSortingRequestedPayload
                {
                    TaskId = _taskItem.Id,
                    SortBy = sortBy,
                    Ascending = ascending
                }));
    }

}
