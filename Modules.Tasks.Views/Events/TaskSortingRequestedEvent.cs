using Prism.Events;

namespace Modules.Tasks.Views.Events;

public class TaskSortingRequestedEvent : PubSubEvent<TaskSortingRequestedPayload>;

public class TaskSortingRequestedPayload
{
    public required int TaskId { get; init; }

    public required SortByProperty SortBy { get; init; }

    public bool Ascending { get; set; }

    public enum SortByProperty
    {
        State = 0,
        CreationDate,
        ModificationDate,
        Content
    }
}
