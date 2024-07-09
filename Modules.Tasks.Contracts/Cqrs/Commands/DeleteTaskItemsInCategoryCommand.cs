using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class DeleteTaskItemsInCategoryCommand : IRequest
{
    public int CategoryId { get; init; }
}
