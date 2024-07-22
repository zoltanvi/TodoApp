using Prism.Events;

namespace Modules.Categories.Contracts.Events;

public class CategoryNameUpdatedEvent : PubSubEvent<CategoryNameUpdatedPayload>;

public class CategoryNameUpdatedPayload
{
    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
}
