using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Services;

public interface IAppSettingsService
{
    void UpdateAppSettingsFromDatabase(IAppSettings appSettings);
    void UpdateDatabaseFromAppSettings(IAppSettings appSettings);
}