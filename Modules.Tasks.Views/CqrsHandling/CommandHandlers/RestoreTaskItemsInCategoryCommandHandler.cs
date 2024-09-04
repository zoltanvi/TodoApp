using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Extensions;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class RestoreTaskItemsInCategoryCommandHandler : IRequestHandler<RestoreTaskItemsInCategoryCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public RestoreTaskItemsInCategoryCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
        _mediator = mediator;
        _eventAggregator = eventAggregator;
    }

    public async Task Handle(RestoreTaskItemsInCategoryCommand request, CancellationToken cancellationToken)
    {
        var deletedTasks = _taskItemRepository.GetDeletedTasksFromCategory(request.CategoryId);
        ArgumentNullException.ThrowIfNull(deletedTasks);
        if (deletedTasks.Count == 0)
        {
            throw new ArgumentException("There is no task in category to restore!");
        }

        var dbCategory = _categoriesRepository.GetCategoryById(request.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        if (dbCategory.IsDeleted)
        {
            await _mediator.Send(new RestoreCategoryCommand { Id = dbCategory.Id }, cancellationToken);
        }

        var query = new TaskInsertPositionQuery
        {
            TaskId = deletedTasks.First().Id,
            PositionChangeReason = PositionChangeReason.Restored
        };

        var newStartIndex = _mediator.Send(query, cancellationToken).Result;

        _taskItemRepository.RestoreTasksInCategory(request.CategoryId, newStartIndex);

        var deletedTaskIds = deletedTasks.Select(x => x.Id).ToHashSet();
        InsertAndFixListOrders(deletedTaskIds, request.CategoryId, newStartIndex, deletedTasks);

        // Notify view
        _eventAggregator.GetEvent<AllTasksInCategoryRestoredEvent>().Publish(request.CategoryId);
    }

    private void InsertAndFixListOrders(
        HashSet<int> deletedTaskIds,
        int requestCategoryId,
        int newStartIndex,
        List<TaskItem> deletedTasks)
    {
        // Filter out the tasks that we want to insert into the correct position
        var otherTasksInCategory = _taskItemRepository
            .GetActiveTasksFromCategory(requestCategoryId)
            .Where(x => !deletedTaskIds.Contains(x.Id))
            .ToList();

        // Insert into the correct position
        otherTasksInCategory.InsertRange(newStartIndex, deletedTasks);

        // Fix list orders
        otherTasksInCategory.SetListOrdersToIndex();

        _taskItemRepository.UpdateTaskListOrders(otherTasksInCategory);
    }
}
