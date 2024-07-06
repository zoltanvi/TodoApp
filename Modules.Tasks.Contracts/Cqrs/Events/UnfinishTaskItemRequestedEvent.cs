using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Events;

public class UnfinishTaskItemRequestedEvent : INotification
{
    public int TaskId { get; set; }
}
