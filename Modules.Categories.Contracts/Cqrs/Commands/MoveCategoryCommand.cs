using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class MoveCategoryCommand : IRequest
{
    public required int CategoryId { get; init; }
    public int? NewParentCategoryId { get; init; }
}
