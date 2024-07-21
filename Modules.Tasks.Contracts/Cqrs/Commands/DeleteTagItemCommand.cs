using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class DeleteTagItemCommand : IRequest
{
    public int TagId { get; set; }
}
