namespace Modules.Categories.Contracts.Cqrs.Events;

public class RestoreCategoryRequestedEvent
{
    public static event Action<RestoreCategoryRequestedEvent>? RestoreCategoryRequested;
    public static void Invoke(RestoreCategoryRequestedEvent obj) => RestoreCategoryRequested?.Invoke(obj);

    public int CategoryId { get; init; }
}
