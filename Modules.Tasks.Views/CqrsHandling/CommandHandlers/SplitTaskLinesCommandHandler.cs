using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.TextEditor.Helpers;
using Modules.Tasks.Services.Extensions;
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

    public Task Handle(SplitTaskLinesCommand request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        List<string> splitContent;

        if (dbTask.IsContentPlainText)
        {
            splitContent = dbTask.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        else
        {
            splitContent = FlowDocumentSplitByLineHelper.SplitByLines(dbTask.Content);
        }

        // The first new listOrder
        var startingListOrder = _mediator.Send(new TaskCreationListOrderQuery { CategoryId = dbTask.CategoryId }, cancellationToken).Result;

        var taskList = new List<TaskItem>();
        foreach (var lineContent in splitContent)
        {
            var task = new TaskItem
            {
                Content = lineContent,
                ContentPreview = dbTask.IsContentPlainText
                    ? lineContent
                    : XmlToPlainTextConverter.ConvertToPlainText(lineContent),
                IsContentPlainText = dbTask.IsContentPlainText,
                CategoryId = dbTask.CategoryId,
                ListOrder = startingListOrder
            };

            startingListOrder++;

            taskList.Add(task);
        }
        
        _taskItemRepository.AddTasks(taskList);

        var idList = taskList.Select(x => x.Id).ToHashSet();

        // Filter out the task that we want to insert into the correct position
        var otherTasksInCategory = _taskItemRepository.GetActiveTasksFromCategory(dbTask.CategoryId)
            .Where(x => !idList.Contains(x.Id))
            .ToList();

        foreach (var taskItem in taskList)
        {
            // Insert into the correct position
            otherTasksInCategory.Insert(taskItem.ListOrder, taskItem);
        }

        // Fix list orders
        otherTasksInCategory.SetListOrdersToIndex();

        _taskItemRepository.UpdateTaskListOrders(otherTasksInCategory);

        _eventAggregator.GetEvent<TaskSplittedByLinesEvent>().Publish(dbTask.CategoryId);

        return Task.CompletedTask;
    }
}
