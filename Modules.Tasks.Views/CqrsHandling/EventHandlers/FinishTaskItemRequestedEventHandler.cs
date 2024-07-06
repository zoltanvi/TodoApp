using MediatR;
using Modules.Tasks.Contracts.Cqrs.Events;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class FinishTaskItemRequestedEventHandler : INotificationHandler<FinishTaskItemRequestedEvent>
{
    public static event Action<FinishTaskItemRequestedEvent>? FinishTaskItemRequested;

    public Task Handle(FinishTaskItemRequestedEvent notification, CancellationToken cancellationToken)
    {
        FinishTaskItemRequested?.Invoke(notification);

        return Task.CompletedTask;
    }
}
