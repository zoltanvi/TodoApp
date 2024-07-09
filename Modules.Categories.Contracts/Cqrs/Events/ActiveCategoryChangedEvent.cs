using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Events;

public class ActiveCategoryChangedEvent : INotification
{
    public required int CategoryId { get; set; }
    public required string CategoryName { get; set; }
}
