using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Events;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class MoveActiveTasksToNewCategoryCommandHandler : IRequestHandler<MoveActiveTasksToNewCategoryCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMediator _mediator;

    public MoveActiveTasksToNewCategoryCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(mediator);

        _taskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
        _eventAggregator = eventAggregator;
        _mediator = mediator;
    }

    public Task Handle(MoveActiveTasksToNewCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.OldCategoryId == request.NewCategoryId)
        {
            return Task.CompletedTask;
        }

        var dbCategory = _categoriesRepository.GetCategoryById(request.NewCategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        // Old category tasks
        var oldCategoryTasks = _taskItemRepository.GetActiveTasksFromCategory(request.OldCategoryId);
        ArgumentNullException.ThrowIfNull(oldCategoryTasks);
        var taskIds = oldCategoryTasks.Select(x => x.Id).ToHashSet();

        var oldPinnedTasks = oldCategoryTasks.Where(x => x.Pinned).ToList();
        var oldNonPinnedTasks = oldCategoryTasks.Where(x => !x.Pinned).ToList();

        // New category tasks
        var newCategoryTasks = _taskItemRepository.GetActiveTasksFromCategory(request.NewCategoryId);
        ArgumentNullException.ThrowIfNull(newCategoryTasks);

        newCategoryTasks = newCategoryTasks
            .Where(x => !taskIds.Contains(x.Id))
            .ToList();

        InsertTasksToList(oldPinnedTasks, newCategoryTasks, request.NewCategoryId, cancellationToken);
        // Move pinned tasks to category
        _taskItemRepository.MoveTasksToCategory(oldPinnedTasks, request.NewCategoryId);

        InsertTasksToList(oldNonPinnedTasks, newCategoryTasks, request.NewCategoryId, cancellationToken);
        // Move non-pinned tasks to category
        _taskItemRepository.MoveTasksToCategory(oldNonPinnedTasks, request.NewCategoryId);

        // Fix list orders
        for (var i = 0; i < newCategoryTasks.Count; i++)
        {
            newCategoryTasks[i].ListOrder = i;
        }

        // Update fixed list orders
        _taskItemRepository.UpdateTaskListOrders(newCategoryTasks);

        // Notify view
        _eventAggregator.GetEvent<TaskItemsCategoryChangedEvent>().Publish(new TaskItemsCategoryChangedPayload
        {
            TaskIds = oldCategoryTasks.Select(x => x.Id).ToList(),
            OldCategoryId = request.OldCategoryId,
            NewCategoryId = request.NewCategoryId
        });

        return Task.CompletedTask;
    }

    private void InsertTasksToList(
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

