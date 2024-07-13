namespace Modules.Tasks.Contracts.Cqrs.Events;

public abstract class BaseTaskItemEvent
{
    public int TaskId { get; init; }
}
