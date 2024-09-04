using Prism.Events;

namespace Modules.Tasks.Services.Events;

public class TaskItemsCategoryChangedEvent : PubSubEvent<TaskItemsCategoryChangedPayload>;

public class TaskItemsCategoryChangedPayload
{
    public required List<int> TaskIds { get; set; }
    public required int OldCategoryId { get; set; }
    public required int NewCategoryId { get; set; }
}