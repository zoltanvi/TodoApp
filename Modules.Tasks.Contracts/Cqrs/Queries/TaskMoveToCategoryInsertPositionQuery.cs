using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class TaskMoveToCategoryInsertPositionQuery : IRequest<int>
{
    public required int TaskId { get; set; }
    public required int CategoryId { get; set; }
}
