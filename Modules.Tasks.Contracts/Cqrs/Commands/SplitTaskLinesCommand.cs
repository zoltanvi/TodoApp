using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class SplitTaskLinesCommand : IRequest
{
    public int TaskId { get; set; }
}
