using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Events;
using Modules.Settings.Services;
using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;

namespace Modules.Settings.Services.PrismSubscribers;

/// <summary>
/// Subscribes to app lifecycle and UI scale Prism events (single pub/sub bus for cross-cutting reactions).
/// Uses instance methods (not lambdas) so Prism keeps strong references. Resolves scoped services per callback.
/// </summary>
public sealed class ApplicationLifecyclePrismSubscriber
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUIScaler _uiScaler;

    public ApplicationLifecyclePrismSubscriber(
        IEventAggregator eventAggregator,
        IServiceScopeFactory scopeFactory,
        IUIScaler uiScaler)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(uiScaler);

        _scopeFactory = scopeFactory;
        _uiScaler = uiScaler;

        eventAggregator.GetEvent<ApplicationOpeningEvent>().Subscribe(OnApplicationOpening);
        eventAggregator.GetEvent<ApplicationClosingEvent>().Subscribe(OnApplicationClosing);
        eventAggregator.GetEvent<UiScaledViewEvent>().Subscribe(OnUiScaled);
    }

    private void OnApplicationOpening()
    {
        using var scope = _scopeFactory.CreateScope();
        var appSettingsService = scope.ServiceProvider.GetRequiredService<IAppSettingsService>();
        appSettingsService.UpdateAppSettingsFromDatabase(AppSettings.Instance);
        _uiScaler.SetScaling(AppSettings.Instance.WindowSettings.Scaling);
    }

    private void OnApplicationClosing()
    {
        using var scope = _scopeFactory.CreateScope();
        var appSettingsService = scope.ServiceProvider.GetRequiredService<IAppSettingsService>();
        appSettingsService.UpdateDatabaseFromAppSettings(AppSettings.Instance);
    }

    private void OnUiScaled(UiScaledViewPayload _)
    {
        if (Math.Abs(_uiScaler.ScaleValue - AppSettings.Instance.WindowSettings.Scaling) < 0.0001) return;

        AppSettings.Instance.WindowSettings.Scaling = _uiScaler.ScaleValue;
        AppSettings.Instance.TaskSettings.OnPropertyChanged(nameof(TaskSettings.FontSize));
        AppSettings.Instance.PageTitleSettings.OnPropertyChanged(nameof(PageTitleSettings.FontSize));
    }
}
