using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Events;

public class RestoreCategoryRequestedEvent : INotification
{
    public int CategoryId { get; set; }
}
