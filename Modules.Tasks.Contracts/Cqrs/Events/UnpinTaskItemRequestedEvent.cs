namespace Modules.Tasks.Contracts.Cqrs.Events;

public class UnpinTaskItemRequestedEvent : BaseTaskItemEvent
{
    public static event Action<UnpinTaskItemRequestedEvent>? UnpinTaskItemRequested;
    public static void Invoke(UnpinTaskItemRequestedEvent obj) => UnpinTaskItemRequested?.Invoke(obj);
}