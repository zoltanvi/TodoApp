using Prism.Events;

namespace Modules.Common.Events;

public class HotkeyPressedCtrlFEvent : PubSubEvent;

public class HotkeyPressedCtrlNEvent : PubSubEvent;

/// <summary>
/// Move keyboard focus to the task page &quot;add new task&quot; field (e.g. Tab from the category list).
/// </summary>
public class FocusTaskPageNewTaskEditorEvent : PubSubEvent;