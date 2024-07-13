namespace Modules.Tasks.Contracts.Cqrs.Events;

public class DeleteTaskItemRequestedEvent : BaseTaskItemEvent
{
    public static event Action<DeleteTaskItemRequestedEvent>? DeleteTaskItemRequested;
    public static void Invoke(DeleteTaskItemRequestedEvent obj) => DeleteTaskItemRequested?.Invoke(obj);
}
