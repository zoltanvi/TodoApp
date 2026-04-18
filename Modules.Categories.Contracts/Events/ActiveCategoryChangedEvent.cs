using Prism.Events;

namespace Modules.Categories.Contracts.Events;

public class ActiveCategoryChangedEvent : PubSubEvent<ActiveCategoryChangedPayload>;

public class ActiveCategoryChangedPayload
{
    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
}
