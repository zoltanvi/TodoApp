using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Views.CqrsHandling.QueryHandlers;

public class TaskDragDropInsertPositionQueryHandler : IRequestHandler<TaskDragDropInsertPositionQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskDragDropInsertPositionQueryHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task<int> Handle(TaskDragDropInsertPositionQuery request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);
        
        var activeTasks = _taskItemRepository
            .GetActiveTasksFromCategory(dbTask.CategoryId)
            .Where(x => x.Id != request.TaskId);

        var stats = activeTasks.Aggregate(
            new { PinnedItemsCount = 0, DoneItemsCount = 0, ActiveTaskCount = 0 },
            (acc, taskItem) => new
            {
                PinnedItemsCount = acc.PinnedItemsCount + (taskItem.Pinned ? 1 : 0),
                DoneItemsCount = acc.DoneItemsCount + (taskItem.IsDone ? 1 : 0),
                ActiveTaskCount = acc.ActiveTaskCount + 1
            });
    
        var pinnedItemsCount = stats.PinnedItemsCount;
        var activeTaskCount = stats.ActiveTaskCount;
        var doneItemsCount = stats.DoneItemsCount;
        var forcedOrder = AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState;

        var newIndex = request.RequestedInsertPosition;

        // If the task is pinned,
        // it must be on top of the list or directly before or after another pinned item
        if (dbTask.Pinned)
        {
            if (newIndex > pinnedItemsCount)
            {
                newIndex = pinnedItemsCount;
            }
        }
        // If the task is not pinned, it must be after the pinned tasks.
        else if (!dbTask.Pinned && newIndex < pinnedItemsCount)
        {
            newIndex = pinnedItemsCount;
        }

        if (forcedOrder)
        {
            // If the task is done,
            // it must be on the bottom of the list or directly before or after another done item
            if (dbTask.IsDone)
            {
                if (newIndex < activeTaskCount - doneItemsCount)
                {
                    newIndex = activeTaskCount - doneItemsCount;
                }
            }
            // If the task is not done, it must be before the finished tasks.
            else if (!dbTask.IsDone)
            {
                if (newIndex > activeTaskCount - doneItemsCount)
                {
                    newIndex = activeTaskCount - doneItemsCount;
                }
            }
        }

        return Task.FromResult(newIndex);
    }
}
