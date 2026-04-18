using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Services.CqrsHandling.QueryHandlers;

public class TaskInsertPositionQueryHandler : IRequestHandler<TaskInsertPositionQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IAppSettings _appSettings;

    public TaskInsertPositionQueryHandler(ITaskItemRepository taskItemRepository, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(appSettings);
        _taskItemRepository = taskItemRepository;
        _appSettings = appSettings;
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
        var forcedOrder = _appSettings.TaskPageSettings.ForceTaskOrderByState;
        var reversedOrder = _appSettings.TaskPageSettings.InsertOrderReversed;
        var newIndex = request.PositionChangeReason switch
        {
            PositionChangeReason.Pinned => 0,
            PositionChangeReason.Unpinned => pinnedItemsCount,
            PositionChangeReason.Done => forcedOrder 
                ? activeTaskCount
                : dbTask.ListOrder < pinnedItemsCount 
                    ? pinnedItemsCount 
                    : dbTask.ListOrder,
            PositionChangeReason.Undone => forcedOrder ? pinnedItemsCount : dbTask.ListOrder,
            PositionChangeReason.Restored => reversedOrder ? activeTaskCount : pinnedItemsCount,
            _ => throw new ArgumentOutOfRangeException(nameof(PositionChangeReason))
        };

        return Task.FromResult(newIndex);
    }
}
