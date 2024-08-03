using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class RestoreTaskItemCommandHandler : IRequestHandler<RestoreTaskItemCommand>
{
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;

    public RestoreTaskItemCommandHandler(
        IMediator mediator,
        IEventAggregator eventAggregator,
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);

        _mediator = mediator;
        _eventAggregator = eventAggregator;
        _taskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
    }

    public async Task Handle(RestoreTaskItemCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbCategory = _categoriesRepository.GetCategoryById(dbTask.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        if (dbCategory.IsDeleted)
        {
            await _mediator.Send(new RestoreCategoryCommand { Id = dbCategory.Id }, cancellationToken);
        }

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Restored
        };

        var newIndex = _mediator.Send(query, cancellationToken).Result;

        _taskItemRepository.RestoreTask(dbTask, newIndex);

        InsertAndFixListOrders(dbTask.Id, dbCategory.Id, newIndex, dbTask);

        _eventAggregator.GetEvent<TaskRestoredEvent>().Publish(new TaskRestoredPayload
        {
            TaskId = dbTask.Id,
            CategoryId = dbCategory.Id
        });
    }

    private void InsertAndFixListOrders(int taskId, int categoryId, int newIndex, TaskItem taskItem)
    {
        var otherTasksInCategory = _taskItemRepository.GetActiveTasksFromCategory(categoryId)
            .Where(x => x.Id != taskId)
            .ToList();

        otherTasksInCategory.Insert(newIndex, taskItem);

        for (var i = 0; i < otherTasksInCategory.Count; i++)
        {
            otherTasksInCategory[i].ListOrder = i;
        }

        _taskItemRepository.UpdateTaskListOrders(otherTasksInCategory);
    }
}
