using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class OpenTagSelectorCommand : IRequest
{
    public int TaskId { get; set; }
}
