using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Events;

public class PinTaskItemRequestedEvent : INotification
{
    public int TaskId { get; set; }
}
