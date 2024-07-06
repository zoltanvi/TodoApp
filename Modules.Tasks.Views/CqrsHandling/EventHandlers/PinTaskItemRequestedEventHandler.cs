using MediatR;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class PinTaskItemRequestedEventHandler : INotificationHandler<PinTaskItemRequestedEvent>
{
    public static event Action<PinTaskItemRequestedEvent>? PinTaskItemRequested;

    public Task Handle(PinTaskItemRequestedEvent notification, CancellationToken cancellationToken)
    {
        PinTaskItemRequested?.Invoke(notification);

        return Task.CompletedTask;
    }
}
