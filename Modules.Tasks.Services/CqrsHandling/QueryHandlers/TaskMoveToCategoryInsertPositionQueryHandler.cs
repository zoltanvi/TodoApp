using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Services.CqrsHandling.QueryHandlers;

/// <summary>
/// Returns the index where the task should be inserted in the new category.
/// The task SHOULD NOT be moved into the new category yet, when this is called.
/// </summary>
public class TaskMoveToCategoryInsertPositionQueryHandler : IRequestHandler<TaskMoveToCategoryInsertPositionQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IAppSettings _appSettings;

    public TaskMoveToCategoryInsertPositionQueryHandler(ITaskItemRepository taskItemRepository, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(appSettings);

        _taskItemRepository = taskItemRepository;
        _appSettings = appSettings;
    }
    public Task<int> Handle(TaskMoveToCategoryInsertPositionQuery request, CancellationToken cancellationToken)
    {
        var dbTask = _taskItemRepository.GetTaskById(request.TaskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        var activeTasks = _taskItemRepository.GetActiveTasksFromCategory(request.CategoryId);

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
        var reversedOrder = _appSettings.TaskPageSettings.InsertOrderReversed;

        var result = dbTask.Pinned ? pinnedItemsCount : (reversedOrder ? activeTaskCount : pinnedItemsCount);

        return Task.FromResult(result);
    }
}
