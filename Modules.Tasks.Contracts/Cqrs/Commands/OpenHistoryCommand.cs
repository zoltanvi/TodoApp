using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class OpenHistoryCommand : IRequest
{
    public int TaskId { get; set; }
}
