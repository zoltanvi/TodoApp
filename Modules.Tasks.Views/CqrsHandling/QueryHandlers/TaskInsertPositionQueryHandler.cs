using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Views.CqrsHandling.QueryHandlers;

public class TaskInsertPositionQueryHandler : IRequestHandler<TaskInsertPositionQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskInsertPositionQueryHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task<int> Handle(TaskInsertPositionQuery request, CancellationToken cancellationToken)
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
        var forcedOrder = AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState;

        var requestedPosition = request.PositionChangeReason switch
        {
            PositionChangeReason.Pinned => 0,
            PositionChangeReason.Unpinned => pinnedItemsCount,
            PositionChangeReason.Done => forcedOrder 
                ? activeTaskCount
                : dbTask.ListOrder < pinnedItemsCount 
                    ? pinnedItemsCount 
                    : dbTask.ListOrder,
            PositionChangeReason.Undone => forcedOrder ? pinnedItemsCount : dbTask.ListOrder,
            _ => throw new ArgumentOutOfRangeException(nameof(PositionChangeReason))
        };

        return Task.FromResult(requestedPosition);
    }
}
