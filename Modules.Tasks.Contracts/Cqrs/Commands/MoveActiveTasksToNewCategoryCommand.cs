using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class MoveActiveTasksToNewCategoryCommand : IRequest
{
    public required int OldCategoryId { get; set; }
    public required int NewCategoryId { get; set; }
}
