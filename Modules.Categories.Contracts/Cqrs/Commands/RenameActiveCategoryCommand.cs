using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RenameActiveCategoryCommand : IRequest<string>
{
    public required string Name { get; init; }
}
