using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class TaskInsertPositionQuery : IRequest<int>
{
    public required int TaskId { get; set; }
    public PositionChangeReason PositionChangeReason { get; set; }
}

public enum PositionChangeReason
{
    Pinned,
    Unpinned,
    Done,
    Undone,
    Restored
}
