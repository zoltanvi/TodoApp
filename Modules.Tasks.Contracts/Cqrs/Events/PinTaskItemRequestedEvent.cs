namespace Modules.Tasks.Contracts.Cqrs.Events;

public class PinTaskItemRequestedEvent : BaseTaskItemEvent
{
    public static event Action<PinTaskItemRequestedEvent>? PinTaskItemRequested;
    public static void Invoke(PinTaskItemRequestedEvent obj) => PinTaskItemRequested?.Invoke(obj);
}