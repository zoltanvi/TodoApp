using Prism.Events;

namespace Modules.Common.Events;

public class UiScaledViewEvent : PubSubEvent<UiScaledViewPayload>;

public class UiScaledViewPayload
{
    public double OldScaleValue { get; set; }
    public double NewScaleValue { get; set; }
}
