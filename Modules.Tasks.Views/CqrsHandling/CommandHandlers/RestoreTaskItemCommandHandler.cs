using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class RestoreTaskItemCommandHandler : IRequestHandler<RestoreTaskItemCommand>
{
    private readonly IMediator _mediator;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;

    public RestoreTaskItemCommandHandler(
        IMediator mediator,
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);

        _mediator = mediator;
        _taskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
    }

    public Task Handle(RestoreTaskItemCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbCategory = _categoriesRepository.GetCategoryById(dbTask.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        if (dbCategory.IsDeleted)
        {
            RestoreCategoryRequestedEvent.Invoke(new RestoreCategoryRequestedEvent { CategoryId = dbCategory.Id });
        }

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Restored
        };

        var newIndex = _mediator.Send(query, cancellationToken).Result;
        
        _taskItemRepository.RestoreTask(dbTask, newIndex);
        
        InsertAndFixListOrders(dbTask.Id, dbCategory.Id, newIndex, dbTask);
        
        TaskRestoredEvent.Invoke(new TaskRestoredEvent{ TaskId = dbTask.Id, CategoryId = dbCategory.Id });

        return Task.CompletedTask;
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
