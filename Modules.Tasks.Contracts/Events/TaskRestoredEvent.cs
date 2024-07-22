using Prism.Events;

namespace Modules.Tasks.Contracts.Events;

public class TaskRestoredEvent : PubSubEvent<TaskRestoredPayload>;

public class TaskRestoredPayload
{
    public required int TaskId { get; init; }
    public required int CategoryId { get; init; }
}
