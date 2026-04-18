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

    public ActiveCategoryNavigationPrismSubscriber(
        IEventAggregator eventAggregator,
        IMainPageNavigationService mainPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);

        _mainPageNavigationService = mainPageNavigationService;

        eventAggregator.GetEvent<ActiveCategoryChangedEvent>().Subscribe(OnActiveCategoryChanged);
    }

    private void OnActiveCategoryChanged(ActiveCategoryChangedPayload payload)
    {
        if (AppSettings.Instance.ApplicationSettings.CloseSideMenuOnPageChange)
        {
            AppSettings.Instance.SessionSettings.SideMenuOpen = false;
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
