using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Services.Events;
using Modules.Tasks.Services.Extensions;
using Prism.Events;

namespace Modules.Tasks.Services.CqrsHandling.CommandHandlers.MoveToCategory;

public abstract class BaseMoveTaskToNewCategoryCommandHandler
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMediator _mediator;

    protected ITaskItemRepository TaskItemRepository { get; }

    protected BaseMoveTaskToNewCategoryCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(mediator);

        TaskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
        _eventAggregator = eventAggregator;
        _mediator = mediator;
    }

    protected abstract List<TaskItem> GetOldCategoryTasks(int oldCategoryId);

    protected virtual void InsertTasksToNewCategory(
        int newCategoryId,
        List<TaskItem> oldCategoryTasks,
        List<TaskItem> newCategoryTasks,
        CancellationToken cancellationToken)
    {
        InsertTasksToList(oldCategoryTasks, newCategoryTasks, newCategoryId, cancellationToken);
        TaskItemRepository.MoveTasksToCategory(oldCategoryTasks, newCategoryId);
    }

    protected Task HandleInternal(
        int oldCategoryId,
        int newCategoryId,
        CancellationToken cancellationToken)
    {
        if (oldCategoryId == newCategoryId)
        {
            return Task.CompletedTask;
        }

        var dbCategory = _categoriesRepository.GetCategoryById(newCategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        var oldCategoryTasks = GetOldCategoryTasks(oldCategoryId);

        var newCategoryTasks = TaskItemRepository.GetActiveTasksFromCategory(newCategoryId);
        ArgumentNullException.ThrowIfNull(newCategoryTasks);

        InsertTasksToNewCategory(newCategoryId, oldCategoryTasks, newCategoryTasks, cancellationToken);

        var stayedInOldCategoryTasks = TaskItemRepository.GetActiveTasksFromCategory(oldCategoryId);
        ArgumentNullException.ThrowIfNull(oldCategoryTasks);

        // Fix list orders in old category
        stayedInOldCategoryTasks.SetListOrdersToIndex();

        // Fix list orders in new category
        newCategoryTasks.SetListOrdersToIndex();

        // Update list orders in old + in new category
        TaskItemRepository.UpdateTaskListOrders(stayedInOldCategoryTasks.Union(newCategoryTasks));

        // Notify view
        _eventAggregator.GetEvent<TaskItemsCategoryChangedEvent>().Publish(new TaskItemsCategoryChangedPayload
        {
            TaskIds = oldCategoryTasks.Select(x => x.Id).ToList(),
            OldCategoryId = oldCategoryId,
            NewCategoryId = newCategoryId
        });

        return Task.CompletedTask;
    }

    protected void InsertTasksToList(
        List<TaskItem> sourceList,
        List<TaskItem> destinationList,
        int destinationCategoryId,
        CancellationToken cancellationToken)
    {
        if (sourceList.Count != 0)
        {
            var startingIndex = _mediator.Send(
                new TaskMoveToCategoryInsertPositionQuery
                {
                    TaskId = sourceList.First().Id,
                    CategoryId = destinationCategoryId
                }, cancellationToken).Result;

            // Insert the pinned tasks into the correct position in local list
            foreach (TaskItem taskItem in sourceList)
            {
                destinationList.Insert(startingIndex, taskItem);

                startingIndex++;
            }
        }
    }
}
