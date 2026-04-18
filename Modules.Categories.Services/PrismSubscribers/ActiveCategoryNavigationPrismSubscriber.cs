using Modules.Categories.Contracts.Events;
using Modules.Common;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;

namespace Modules.Categories.Services.PrismSubscribers;

/// <summary>
/// Navigates main content when active category changes (Prism pub/sub).
/// </summary>
public sealed class ActiveCategoryNavigationPrismSubscriber
{
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly IAppSettings _appSettings;

    public ActiveCategoryNavigationPrismSubscriber(
        IEventAggregator eventAggregator,
        IMainPageNavigationService mainPageNavigationService,
        IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(appSettings);

        _mainPageNavigationService = mainPageNavigationService;
        _appSettings = appSettings;

        eventAggregator.GetEvent<ActiveCategoryChangedEvent>().Subscribe(OnActiveCategoryChanged);
    }

    private void OnActiveCategoryChanged(ActiveCategoryChangedPayload payload)
    {
        if (_appSettings.ApplicationSettings.CloseSideMenuOnPageChange)
        {
            _appSettings.SessionSettings.SideMenuOpen = false;
        }

        if (payload.CategoryId == Constants.RecycleBinCategoryId)
        {
            _mainPageNavigationService.NavigateTo<IRecycleBinPage>();
        }
        else
        {
            _mainPageNavigationService.NavigateTo<ITaskPage>();
        }
    }
}
