namespace Modules.Categories.Contracts.Cqrs.Events;

public class CategoryDeletedEvent
{
    public static event Action<CategoryDeletedEvent>? CategoryDeleted;
    public static void Invoke(CategoryDeletedEvent obj) => CategoryDeleted?.Invoke(obj);
    public required int CategoryId { get; init; }
}