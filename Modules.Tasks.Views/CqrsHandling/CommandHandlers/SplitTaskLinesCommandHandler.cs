using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Services.Extensions;
using Modules.Tasks.Views.Extensions;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class SplitTaskLinesCommandHandler : IRequestHandler<SplitTaskLinesCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public SplitTaskLinesCommandHandler(
        ITaskItemRepository taskItemRepository,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _mediator = mediator;
        _eventAggregator = eventAggregator;
    }

    public async Task Handle(SplitTaskLinesCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var splitContent = dbTask.Content
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        var startingListOrder = await _mediator.Send(new TaskCreationListOrderQuery { CategoryId = dbTask.CategoryId }, cancellationToken);

        var taskList = new List<TaskItem>();
        foreach (var lineContent in splitContent)
        {
            var task = new TaskItem
            {
                Content = lineContent,
                ContentPreview = lineContent,
                CategoryId = dbTask.CategoryId,
                ListOrder = startingListOrder
            };

            startingListOrder++;

            taskList.Add(task);
        }

        _taskItemRepository.DeleteTask(dbTask);
        _taskItemRepository.AddTasks(taskList);

        var idList = taskList.Select(x => x.Id).ToHashSet();

        var otherTasksInCategory = _taskItemRepository.GetActiveTasksFromCategory(dbTask.CategoryId)
            .Where(x => !idList.Contains(x.Id))
            .ToList();

        foreach (var taskItem in taskList)
        {
            otherTasksInCategory.Insert(taskItem.ListOrder, taskItem);
        }

        otherTasksInCategory.SetListOrdersToIndex();

        _taskItemRepository.UpdateTaskListOrders(otherTasksInCategory);

        _eventAggregator.GetEvent<TaskSplittedByLinesEvent>().Publish(dbTask.CategoryId);
    }
}
