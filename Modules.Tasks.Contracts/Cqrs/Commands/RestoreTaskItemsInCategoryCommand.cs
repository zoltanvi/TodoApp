using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class RestoreTaskItemsInCategoryCommand : IRequest
{
    public int CategoryId { get; init; }
}
