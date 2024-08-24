using Prism.Events;

namespace Modules.Tasks.Views.Events;

public class TaskResetRequestedEvent : PubSubEvent<TaskResetRequestedPayload>;

public class TaskResetRequestedPayload
{
    public required Subject ResetSubject { get; set; }

    public enum Subject
    {
        State,
        AllColors,
        MarkerColor,
        BackgroundColor,
        BorderColor,
        Tag
    }
}