using MediatR;

namespace Modules.Common.Cqrs.Events;

public class UiScaledEvent : INotification
{
    public double OldScaleValue { get; set; }
    public double NewScaleValue { get; set; }
}
