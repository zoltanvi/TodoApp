using Prism.Events;

namespace Modules.Tasks.Views.Events;

public class TaskItemCheckedEvent : PubSubEvent<int>;