using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Extensions;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class MoveTaskToNewCategoryCommandHandler : IRequestHandler<MoveTaskToNewCategoryCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMediator _mediator;

    public MoveTaskToNewCategoryCommandHandler(
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

    public Task Handle(MoveTaskToNewCategoryCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbCategory = _categoriesRepository.GetCategoryById(request.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        if (request.CategoryId == dbTask.CategoryId)
        {
            return Task.CompletedTask;
        }

        var oldCategoryId = dbTask.CategoryId;
        var newCategoryId = request.CategoryId;

        // The moved task should be inserted to the correct position
        var newListOrder = _mediator.Send(
            new TaskMoveToCategoryInsertPositionQuery
            {
                TaskId = dbTask.Id,
                CategoryId = newCategoryId
            }, cancellationToken).Result;

        // Filter out the task that we want to insert into the correct position
        var newCategoryTasks = _taskItemRepository.GetActiveTasksFromCategory(newCategoryId)
            .Where(x => x.Id != dbTask.Id)
            .ToList();

        // Insert into the correct position in local list
        newCategoryTasks.Insert(newListOrder, dbTask);
        // Move to category
        _taskItemRepository.MoveTaskToCategory(dbTask, newCategoryId);

        var oldCategoryTasks = _taskItemRepository.GetActiveTasksFromCategory(oldCategoryId).ToList();
        ArgumentNullException.ThrowIfNull(oldCategoryTasks);

        // Fix list orders in old category
        oldCategoryTasks.SetListOrdersToIndex();

        // Fix list orders in new category
        newCategoryTasks.SetListOrdersToIndex();

        // Update list orders in old + in new category
        _taskItemRepository.UpdateTaskListOrders(oldCategoryTasks.Union(newCategoryTasks));

        // Notify view
        _eventAggregator.GetEvent<TaskItemCategoryChangedEvent>().Publish(
            new TaskItemCategoryChangedPayload
            {
                TaskId = dbTask.Id,
                OldCategoryId = oldCategoryId,
                NewCategoryId = newCategoryId
            });

        return Task.CompletedTask;
    }
}

