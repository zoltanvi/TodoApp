using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Events;

public class UnpinTaskItemRequestedEvent : INotification
{
    public int TaskId { get; set; }
}
