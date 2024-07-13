namespace Modules.Tasks.Contracts.Cqrs.Events;

public class FinishTaskItemRequestedEvent : BaseTaskItemEvent
{
    public static event Action<FinishTaskItemRequestedEvent>? FinishTaskItemRequested;
    public static void Invoke(FinishTaskItemRequestedEvent obj) => FinishTaskItemRequested?.Invoke(obj);

}