using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class MoveCompletedTasksToNewCategoryCommand : IRequest
{
    public required int OldCategoryId { get; set; }
    public required int NewCategoryId { get; set; }
}
