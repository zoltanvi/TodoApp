using MediatR;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class UnpinTaskItemRequestedEventHandler : INotificationHandler<UnpinTaskItemRequestedEvent>
{
    public static event Action<UnpinTaskItemRequestedEvent>? UnpinTaskItemRequested;

    public Task Handle(UnpinTaskItemRequestedEvent notification, CancellationToken cancellationToken)
    {
        UnpinTaskItemRequested?.Invoke(notification);

        return Task.CompletedTask;
    }
}
