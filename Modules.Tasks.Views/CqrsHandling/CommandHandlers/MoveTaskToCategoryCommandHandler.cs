using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Views.Events;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class MoveTaskToCategoryCommandHandler : IRequestHandler<MoveTaskToCategoryCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMediator _mediator;

    public MoveTaskToCategoryCommandHandler(
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

    public Task Handle(MoveTaskToCategoryCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbCategory = _categoriesRepository.GetCategoryById(request.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        if (request.CategoryId == dbTask.CategoryId)
        {
            throw new InvalidOperationException("The source and destination category is the same.");
        }

        var oldCategoryId = dbTask.CategoryId;
        var newCategoryId = request.CategoryId;

        // The moved task should be inserted to the correct position
        var newListOrder = _mediator.Send(new TaskCreationListOrderQuery { CategoryId = newCategoryId }, cancellationToken).Result;

        // Filter out the task that we want to insert into the correct position
        var otherTasksInCategory = _taskItemRepository.GetActiveTasksFromCategory(newCategoryId)
            .Where(x => x.Id != dbTask.Id)
            .ToList();

        // Insert into the correct position in local list
        otherTasksInCategory.Insert(newListOrder, dbTask);

        // Fix list orders
        for (var i = 0; i < otherTasksInCategory.Count; i++)
        {
            otherTasksInCategory[i].ListOrder = i;
        }

        // Move to category
        _taskItemRepository.MoveTaskToCategory(dbTask, newCategoryId);

        // Update fixed list orders
        _taskItemRepository.UpdateTaskListOrders(otherTasksInCategory);

        // Notify view
        _eventAggregator.GetEvent<TaskItemCategoryChangedEvent>().Publish(new TaskItemCategoryChangedPayload
        {
            TaskId = dbTask.Id,
            OldCategoryId = oldCategoryId,
            NewCategoryId = newCategoryId
        });

        return Task.CompletedTask;
    }
}

