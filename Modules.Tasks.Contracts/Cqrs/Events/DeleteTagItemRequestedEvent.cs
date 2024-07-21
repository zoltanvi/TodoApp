namespace Modules.Tasks.Contracts.Cqrs.Events;

public class DeleteTagItemRequestedEvent
{
    public static event Action<DeleteTagItemRequestedEvent>? DeleteTagItemRequested;
    public static void Invoke(DeleteTagItemRequestedEvent obj) => DeleteTagItemRequested?.Invoke(obj);
 
    public int TagId { get; set; }
}
