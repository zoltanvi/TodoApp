using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Views.Cqrs.EventHandlers;

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

    // TODO: check
    //private void WindowSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    _appSettings.WindowSettings.PropertyChanged += WindowSettings_PropertyChanged;

    //    if (e.PropertyName != nameof(WindowSettings.Scaling)) return;
    //    if (UIScaler.StaticScaleValue == _appSettings.WindowSettings.Scaling) return;

    //    _uiScaler.SetScaling(_appSettings.WindowSettings.Scaling);
    //}
}
