using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class RestoreTaskItemVersionCommand : IRequest
{
    public required int TaskId { get; init; }
    public required int VersionId { get; init; }
}
