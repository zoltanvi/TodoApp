using Prism.Events;

namespace Modules.Tasks.Views.Events;

public class TagsChangedOnTaskItemEvent : PubSubEvent<int>;