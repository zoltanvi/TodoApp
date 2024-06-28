using MediatR;
using Modules.Categories.Contracts.Cqrs.Events;

namespace Modules.Categories.Services.CqrsHandling.EventHandlers;

public class CategoryNameUpdatedEventHandler : INotificationHandler<CategoryNameUpdatedEvent>
{
    public static event Action<CategoryNameUpdatedEvent>? CategoryNameUpdated;
    public Task Handle(CategoryNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        CategoryNameUpdated?.Invoke(notification);
        
        return Task.CompletedTask;
    }
}
