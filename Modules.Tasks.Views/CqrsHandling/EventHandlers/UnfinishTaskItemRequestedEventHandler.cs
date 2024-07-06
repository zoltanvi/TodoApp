using MediatR;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class UnfinishTaskItemRequestedEventHandler : INotificationHandler<UnfinishTaskItemRequestedEvent>
{
    public static event Action<UnfinishTaskItemRequestedEvent>? UnfinishTaskItemRequested;

    public Task Handle(UnfinishTaskItemRequestedEvent notification, CancellationToken cancellationToken)
    {
        UnfinishTaskItemRequested?.Invoke(notification);

        return Task.CompletedTask;
    }
}
