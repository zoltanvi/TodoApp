using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class RestoreTaskItemCommand : IRequest
{
    public int TaskId { get; set; }
}
