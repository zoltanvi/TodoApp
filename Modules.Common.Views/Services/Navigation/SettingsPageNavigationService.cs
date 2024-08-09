using Modules.Common.Services.Navigation;

namespace Modules.Common.Views.Services.Navigation;

public class SettingsPageNavigationService : NavigationService, ISettingsPageNavigationService
{
    public SettingsPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
