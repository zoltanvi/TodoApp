using Modules.Categories.Contracts.Events;
using Modules.Common;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.RecycleBin.Views.Pages;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Views.Pages;
using Modules.Tasks.Views.Services;
using Prism.Events;

namespace TodoApp;

/// <summary>
/// Single subscriber for <see cref="ActiveCategoryChangedEvent"/> with explicit ordering:
/// (1) task view editor state, (2) side menu session flag, (3) main frame navigation.
/// Replaces separate subscribers whose relative order previously depended on DI registration order.
/// </summary>
public sealed class ActiveCategoryChangedCoordinator
{
    public ActiveCategoryChangedCoordinator(
        IEventAggregator eventAggregator,
        OneEditorOpenService oneEditorOpenService,
        IMainPageNavigationService mainPageNavigationService,
        IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(appSettings);

        _oneEditorOpenService = oneEditorOpenService;
        _mainPageNavigationService = mainPageNavigationService;
        _appSettings = appSettings;

        eventAggregator.GetEvent<ActiveCategoryChangedEvent>().Subscribe(OnActiveCategoryChanged);
    }

    private readonly OneEditorOpenService _oneEditorOpenService;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly IAppSettings _appSettings;

    private void OnActiveCategoryChanged(ActiveCategoryChangedPayload payload)
    {
        _oneEditorOpenService.EditModeWithoutTask();

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
