using Prism.Events;

namespace Modules.Tasks.Views.Events;

public class TaskDeleteAllRequestedEvent : PubSubEvent<TaskDeleteAllRequestedPayload>;

public class TaskDeleteAllRequestedPayload
{
    public Mode DeleteMode { get; set; }

    public enum Mode
    {
        All,
        Incomplete,
        Completed
    }
}