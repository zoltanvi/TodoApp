using Prism.Events;

namespace Modules.Tasks.Services.Events;

public class TaskItemVersionRestoredEvent : PubSubEvent<int>;