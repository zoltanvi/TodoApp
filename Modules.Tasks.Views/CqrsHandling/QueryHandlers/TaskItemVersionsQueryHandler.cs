using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Views.CqrsHandling.QueryHandlers;

public class TaskItemVersionsQueryHandler : IRequestHandler<TaskItemVersionsQuery, List<TaskItemVersion>>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskItemVersionsQueryHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task<List<TaskItemVersion>> Handle(TaskItemVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = _taskItemRepository.GetTaskItemVersions(request.TaskId);
        return Task.FromResult(versions);
    }
}
