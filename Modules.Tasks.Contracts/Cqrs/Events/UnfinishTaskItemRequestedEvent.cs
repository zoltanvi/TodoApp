namespace Modules.Tasks.Contracts.Cqrs.Events;

public class UnfinishTaskItemRequestedEvent : BaseTaskItemEvent
{
    public static event Action<UnfinishTaskItemRequestedEvent>? UnfinishTaskItemRequested;
    public static void Invoke(UnfinishTaskItemRequestedEvent obj) => UnfinishTaskItemRequested?.Invoke(obj);
}