using Modules.Common.Views.Services;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Services;

namespace Modules.Settings.Views.Services;

public class AppSettingsAutoSaveService : IAppSettingsAutoSaveService
{
    private readonly IAppSettingsService _appSettingsService;

    public AppSettingsAutoSaveService(IAppSettingsService appSettingsService)
    {
        ArgumentNullException.ThrowIfNull(appSettingsService);
        _appSettingsService = appSettingsService;
    }

    public void StartService()
    {
        TimerService.Instance.CreateTimer(TimeSpan.FromSeconds(3), TickEventHandler, start: true);
    }

    private void TickEventHandler(object? sender, EventArgs e)
    {
        if (AppSettings.Instance.IsDirty())
        {
            _appSettingsService.UpdateDatabaseFromAppSettings(AppSettings.Instance);
            AppSettings.Instance.Clean();
        }
    }
}
