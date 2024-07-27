using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Services.CqrsHandling.EventHandlers;

public class UiScaledEventHandler : INotificationHandler<UiScaledEvent>
{
    private readonly IUIScaler _uiScaler;

    public UiScaledEventHandler(IUIScaler uiScaler)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);

        _uiScaler = uiScaler;
    }

    public Task Handle(UiScaledEvent notification, CancellationToken cancellationToken)
    {
        if (Math.Abs(_uiScaler.ScaleValue - AppSettings.Instance.WindowSettings.Scaling) < 0.0001) return Task.CompletedTask;

        AppSettings.Instance.WindowSettings.Scaling = _uiScaler.ScaleValue;

        // This is needed to trigger the font size update (fontSizeScaler)
        AppSettings.Instance.TaskSettings.OnPropertyChanged(nameof(TaskSettings.FontSize));
        AppSettings.Instance.PageTitleSettings.OnPropertyChanged(nameof(PageTitleSettings.FontSize));

        return Task.CompletedTask;
    }
}
