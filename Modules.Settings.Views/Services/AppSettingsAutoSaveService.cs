using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Views.Services;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Services;

namespace Modules.Settings.Views.Services;

public class AppSettingsAutoSaveService : IAppSettingsAutoSaveService
{
    private readonly IServiceProvider _serviceProvider;

    public AppSettingsAutoSaveService(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    public void StartService()
    {
        TimerService.Instance.CreateTimer(TimeSpan.FromSeconds(3), TickEventHandler, start: true);
    }

    private void TickEventHandler(object? sender, EventArgs e)
    {
        if (AppSettings.Instance.IsDirty())
        {
            using var scope = _serviceProvider.CreateScope();
            var appSettingsService = scope.ServiceProvider.GetRequiredService<IAppSettingsService>();
            appSettingsService.UpdateDatabaseFromAppSettings(AppSettings.Instance);
            AppSettings.Instance.Clean();
        }
    }
}
