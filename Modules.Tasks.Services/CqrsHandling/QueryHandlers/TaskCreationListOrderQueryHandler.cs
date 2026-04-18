using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Services.CqrsHandling.QueryHandlers;

public class TaskCreationListOrderQueryHandler : IRequestHandler<TaskCreationListOrderQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IAppSettings _appSettings;

    public TaskCreationListOrderQueryHandler(ITaskItemRepository taskItemRepository, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(appSettings);
        _taskItemRepository = taskItemRepository;
        _appSettings = appSettings;
    }

    public Task<int> Handle(TaskCreationListOrderQuery request, CancellationToken cancellationToken)
    {
        var activeTasks = _taskItemRepository.GetActiveTasksFromCategory(request.CategoryId);

        var newIndex = _appSettings.TaskPageSettings.InsertOrderReversed
            ? activeTasks.Count
            : activeTasks.Count(x => x.Pinned);

        return Task.FromResult(newIndex);
    }
}
