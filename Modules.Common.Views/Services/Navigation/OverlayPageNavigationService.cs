using Modules.Common.Services.Navigation;

namespace Modules.Common.Views.Services.Navigation;

public class OverlayPageNavigationService : NavigationService, IOverlayPageNavigationService
{
    public OverlayPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
