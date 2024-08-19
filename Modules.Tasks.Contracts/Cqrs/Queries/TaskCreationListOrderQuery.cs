using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class TaskCreationListOrderQuery : IRequest<int>
{
    public required int CategoryId { get; set; }
}
