using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Views.Events;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class RestoreTaskItemVersionCommandHandler : IRequestHandler<RestoreTaskItemVersionCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IEventAggregator _eventAggregator;

    public RestoreTaskItemVersionCommandHandler(
        ITaskItemRepository taskItemRepository,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _eventAggregator = eventAggregator;
    }

    public Task Handle(RestoreTaskItemVersionCommand request, CancellationToken cancellationToken)
    {
        var updatedTask = _taskItemRepository.RestoreTaskToVersion(request.TaskId, request.VersionId);

        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Publish(updatedTask.Id);

        return Task.CompletedTask;
    }
}
