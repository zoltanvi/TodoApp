using Prism.Events;

namespace Modules.Tasks.Services.Events;

public class TaskItemCategoryChangedEvent : PubSubEvent<TaskItemCategoryChangedPayload>;

public class TaskItemCategoryChangedPayload
{
    public required int TaskId { get; set; }
    public required int OldCategoryId { get; set; }
    public required int NewCategoryId { get; set; }
}