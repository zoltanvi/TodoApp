using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class MoveTaskToNewCategoryCommand : IRequest
{
    public required int TaskId { get; set; }
    public required int CategoryId { get; set; }
}