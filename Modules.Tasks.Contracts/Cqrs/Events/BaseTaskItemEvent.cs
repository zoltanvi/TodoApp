using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Events;

public abstract class BaseTaskItemEvent : INotification
{
    public int TaskId { get; init; }
}
