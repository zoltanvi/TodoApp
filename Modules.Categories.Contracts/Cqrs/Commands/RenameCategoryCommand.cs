using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RenameCategoryCommand : IRequest<string>
{
    public required int CategoryId { get; init; }
    public required string Name { get; init; }
}
