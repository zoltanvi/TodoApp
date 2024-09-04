using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;

namespace Modules.Tasks.Services.CqrsHandling.CommandHandlers;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public UpdateTaskCommandHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        _taskItemRepository.UpdateTaskItem(request.Task);

        return Task.CompletedTask;
    }
}
