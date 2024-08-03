using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RestoreCategoryCommand : IRequest
{
    public int Id { get; set; }
}
