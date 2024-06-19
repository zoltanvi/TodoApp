using Modules.Common.Services.Navigation;

namespace Modules.Common.Views.Services.Navigation;

public class SideMenuPageNavigationService : NavigationService, ISideMenuPageNavigationService
{
    public SideMenuPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
