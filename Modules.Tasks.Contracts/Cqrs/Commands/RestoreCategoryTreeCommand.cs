using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class RestoreCategoryTreeCommand : IRequest
{
    public int RootCategoryId { get; init; }
}
