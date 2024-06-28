using MediatR;
using Modules.Common.Cqrs.Events;

namespace TodoApp.CqrsHandling.EventHandlers;

public class UiScaledEventSecondHandler : INotificationHandler<UiScaledEvent>
{
    public static event Action<UiScaledEvent>? UiScaled;

    public Task Handle(UiScaledEvent notification, CancellationToken cancellationToken)
    {
        UiScaled?.Invoke(notification);

        return Task.CompletedTask;
    }
}