using MediatR;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class DeleteTaskItemRequestedEventHandler : INotificationHandler<DeleteTaskItemRequestedEvent>
{
    public static event Action<DeleteTaskItemRequestedEvent>? DeleteTaskItemRequestedEvent;

    public Task Handle(DeleteTaskItemRequestedEvent notification, CancellationToken cancellationToken)
    {
        DeleteTaskItemRequestedEvent?.Invoke(notification);

        return Task.CompletedTask;
    }
}
