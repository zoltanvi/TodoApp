using Prism.Events;

namespace Modules.Categories.Views.Events;

public class CategoryMakeSubcategoryPayload
{
    public required int CategoryId { get; init; }
    public required int NewParentCategoryId { get; init; }
}

public class CategoryMakeSubcategoryClickedEvent : PubSubEvent<CategoryMakeSubcategoryPayload>;
