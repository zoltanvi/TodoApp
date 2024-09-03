using MediatR;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Views.Controls.ContextMenu;
using Modules.Tasks.Views.Controls.TaskItemView;
using Modules.Tasks.Views.Events;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
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

        IsDoneModifiedCommand = new RelayCommand(HandleIsDoneModified);

        EditItemCommand = new RelayCommand(_taskItemInternal.EditItem);
        ToggleIsDoneCommand = new RelayCommand(() =>
        {
            _taskItem.IsDone ^= true;
           HandleIsDoneModified();
        });

        PinItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Publish(_taskItem.Id));
        UnpinItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Publish(_taskItem.Id));
        DeleteItemCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Publish(_taskItem.Id));
        ToggleDetailsCommand = new RelayCommand(() => _taskItem.DetailsVisible ^= true);
        ShowTagSelectorCommand = new RelayCommand(() => _mediator.Send(new OpenTagSelectorCommand { TaskId = _taskItem.Id }));
        ShowHistoryCommand = new RelayCommand(() => _mediator.Send(new OpenHistoryCommand { TaskId = _taskItem.Id }));
        SwitchFormattedPlainTextModeCommand = new RelayCommand(() =>
        {
            _taskItem.Content.IsPlainTextMode ^= true;
            _taskItemInternal.UpdateTask();
            _taskItemInternal.UpdateHistory();
        });

        MoveToTopCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemMoveToTopClickedEvent>().Publish(_taskItem.Id));
        MoveToBottomCommand = new RelayCommand(() => _eventAggregator.GetEvent<TaskItemMoveToBottomClickedEvent>().Publish(_taskItem.Id));

        SplitLinesCommand = new RelayCommand(() => _mediator.Send(new SplitTaskLinesCommand { TaskId = _taskItem.Id }));

        SortByStateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.State);
        SortByCreationDateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.CreationDate, true);
        SortByCreationDateDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.CreationDate);
        SortByModificationDateCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.ModificationDate, true);
        SortByModificationDateDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.ModificationDate);
        SortByContentCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.Content, true);
        SortByContentDescCommand = CreateSortCommand(TaskSortingRequestedPayload.SortByProperty.Content);

        ResetAllStatesCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.State);
        ResetAllColorsCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.AllColors);
        ResetMarkerColorsCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.MarkerColor);
        ResetBackgroundColorsCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.BackgroundColor);
        ResetBorderColorsCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.BorderColor);
        ResetTagsCommand = CreateResetCommand(TaskResetRequestedPayload.Subject.Tag);

        DeleteAllCommand = CreateDeleteAllCommand(TaskDeleteAllRequestedPayload.Mode.All);
        DeleteCompletedCommand = CreateDeleteAllCommand(TaskDeleteAllRequestedPayload.Mode.Completed);
        DeleteIncompleteCommand = CreateDeleteAllCommand(TaskDeleteAllRequestedPayload.Mode.Incomplete);

        MoveToCategoryCommand = new RelayParameterizedCommand<MoveToCategoryViewModel>(MoveToCategory);
        MoveAllToCategoryCommand = new RelayParameterizedCommand<MoveToCategoryViewModel>(MoveAllToCategory);
    }

    private void MoveToCategory(MoveToCategoryViewModel viewModel) => 
        _mediator.Send(new MoveTaskToNewCategoryCommand { TaskId = _taskItem.Id, CategoryId = viewModel.Id });

    private void MoveAllToCategory(MoveToCategoryViewModel viewModel)
    {
        _mediator.Send(new MoveActiveTasksToNewCategoryCommand { OldCategoryId = _taskItem.CategoryId , NewCategoryId = viewModel.Id });
    }

    // TODO: cache
    public ObservableCollection<MoveToCategoryViewModel> InactiveCategories
    {
        get
        {
            var inactiveCategoryInfos = _mediator.Send(new GetInactiveCategoriesQuery()).Result;
            var inactiveCategories =
                new ObservableCollection<MoveToCategoryViewModel>(inactiveCategoryInfos.Select(x =>
                    new MoveToCategoryViewModel { Id = x.Id, Name = x.Name }));

            return inactiveCategories;
        }
    }

    public ICommand IsDoneModifiedCommand { get; set; }
    public ICommand EditItemCommand { get; }
    public ICommand ToggleIsDoneCommand { get; }
    public ICommand OpenReminderCommand { get; }
    public ICommand PinItemCommand { get; }
    public ICommand UnpinItemCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public ICommand ToggleDetailsCommand { get; }
    public ICommand ShowTagSelectorCommand { get; }
    public ICommand ShowHistoryCommand { get; }
    public ICommand SwitchFormattedPlainTextModeCommand { get; set; }

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

    // Reset all
    public ICommand ResetAllStatesCommand { get; }
    public ICommand ResetAllColorsCommand { get; }
    public ICommand ResetMarkerColorsCommand { get; }
    public ICommand ResetBackgroundColorsCommand { get; }
    public ICommand ResetBorderColorsCommand { get; }
    public ICommand ResetTagsCommand { get; }

    // Delete all
    public ICommand DeleteAllCommand { get; }
    public ICommand DeleteCompletedCommand { get; }
    public ICommand DeleteIncompleteCommand { get; }

    private void HandleIsDoneModified()
    {
        if (_taskItem.IsDone)
        {
            _eventAggregator.GetEvent<TaskItemCheckedEvent>().Publish(_taskItem.Id);
        }
        else
        {
            _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Publish(_taskItem.Id);
        }
    }

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

    private ICommand CreateResetCommand(TaskResetRequestedPayload.Subject subject)
    {
        return new RelayCommand(() =>
            _eventAggregator.GetEvent<TaskResetRequestedEvent>()
                .Publish(new TaskResetRequestedPayload { ResetSubject = subject }));
    }

    private ICommand CreateDeleteAllCommand(TaskDeleteAllRequestedPayload.Mode mode)
    {
        return new RelayCommand(() =>
            _eventAggregator.GetEvent<TaskDeleteAllRequestedEvent>()
                .Publish(new TaskDeleteAllRequestedPayload { DeleteMode = mode }));
    }
}
