using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Events;

public class FinishTaskItemRequestedEvent : INotification
{
    public int TaskId { get; set; }
}
