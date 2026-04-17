using Prism.Events;

namespace Modules.Categories.Contracts.Events;

public class CategoryMovedEvent : PubSubEvent<CategoryMovedPayload>;

public class CategoryMovedPayload
{
    public required int CategoryId { get; init; }
    public int? OldParentCategoryId { get; init; }
    public int? NewParentCategoryId { get; init; }
}
