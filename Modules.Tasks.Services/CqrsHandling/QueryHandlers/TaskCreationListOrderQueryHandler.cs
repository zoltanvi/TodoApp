using MediatR;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Services.CqrsHandling.QueryHandlers;

public class TaskCreationListOrderQueryHandler : IRequestHandler<TaskCreationListOrderQuery, int>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskCreationListOrderQueryHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task<int> Handle(TaskCreationListOrderQuery request, CancellationToken cancellationToken)
    {
        var activeTasks = _taskItemRepository.GetActiveTasksFromCategory(request.CategoryId);

        var newIndex = AppSettings.Instance.TaskPageSettings.InsertOrderReversed
            ? activeTasks.Count
            : activeTasks.Count(x => x.Pinned);

        return Task.FromResult(newIndex);
    }
}
