using MediatR;
using Modules.Categories.Contracts.Cqrs.Events;

namespace Modules.Categories.Services.CqrsHandling.EventHandlers;

public class RestoreCategoryRequestedEventHandler : INotificationHandler<RestoreCategoryRequestedEvent>
{
    public static event Action<RestoreCategoryRequestedEvent>? RestoreCategoryRequested;

    public Task Handle(RestoreCategoryRequestedEvent notification, CancellationToken cancellationToken)
    {
        RestoreCategoryRequested?.Invoke(notification);
        return Task.CompletedTask;
    }
}