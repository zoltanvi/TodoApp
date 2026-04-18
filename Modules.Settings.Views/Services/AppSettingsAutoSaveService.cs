using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Views.Services;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Services;

namespace Modules.Settings.Views.Services;

public class AppSettingsAutoSaveService : IAppSettingsAutoSaveService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppSettings _appSettings;

    public AppSettingsAutoSaveService(IServiceProvider serviceProvider, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(appSettings);
        _serviceProvider = serviceProvider;
        _appSettings = appSettings;
    }

    public void StartService()
    {
        TimerService.Instance.CreateTimer(TimeSpan.FromSeconds(3), TickEventHandler, start: true);
    }

    private void TickEventHandler(object? sender, EventArgs e)
    {
        if (_appSettings.IsDirty())
        {
            using var scope = _serviceProvider.CreateScope();
            var appSettingsService = scope.ServiceProvider.GetRequiredService<IAppSettingsService>();
            appSettingsService.UpdateDatabaseFromAppSettings(_appSettings);
            _appSettings.Clean();
        }
    }
}
