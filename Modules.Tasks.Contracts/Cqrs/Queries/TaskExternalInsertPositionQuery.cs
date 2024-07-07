using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class TaskExternalInsertPositionQuery : IRequest<int>
{
    public required int TaskId { get; init; }
    public required int RequestedInsertPosition { get; init; }
}
