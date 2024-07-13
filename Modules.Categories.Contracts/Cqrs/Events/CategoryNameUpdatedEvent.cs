namespace Modules.Categories.Contracts.Cqrs.Events;

public class CategoryNameUpdatedEvent
{
    public static event Action<CategoryNameUpdatedEvent>? CategoryNameUpdated;
    public static void Invoke(CategoryNameUpdatedEvent obj) => CategoryNameUpdated?.Invoke(obj);
    
    public required int Id { get; init; }
    public required string CategoryName { get; init; }
}
