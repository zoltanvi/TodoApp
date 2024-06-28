using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Events;

public class CategoryNameUpdatedEvent : INotification
{
    public required int Id { get; init; }
    public required string CategoryName { get; init; }
}
