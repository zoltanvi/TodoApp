namespace Modules.Tasks.Contracts.Cqrs.Events;

public class TaskRestoredEvent : BaseTaskItemEvent
{
    public static event Action<TaskRestoredEvent>? TaskRestored;
    public static void Invoke(TaskRestoredEvent obj) => TaskRestored?.Invoke(obj);
    
    public required int CategoryId { get; set; }
}
