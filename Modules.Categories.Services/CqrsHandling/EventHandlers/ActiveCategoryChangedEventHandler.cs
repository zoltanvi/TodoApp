using MediatR;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Common;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.CqrsHandling.EventHandlers;

public class ActiveCategoryChangedEventHandler : INotificationHandler<ActiveCategoryChangedEvent>
{
    private readonly IMainPageNavigationService _mainPageNavigationService;

    public ActiveCategoryChangedEventHandler(IMainPageNavigationService mainPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        _mainPageNavigationService = mainPageNavigationService;
    }

    public Task Handle(ActiveCategoryChangedEvent notification, CancellationToken cancellationToken)
    {
        if (AppSettings.Instance.ApplicationSettings.CloseSideMenuOnPageChange)
        {
            AppSettings.Instance.SessionSettings.SideMenuOpen = false;
        }

        if (notification.CategoryId == Constants.RecycleBinCategoryId)
        {
            _mainPageNavigationService.NavigateTo<IRecycleBinPage>();
        }
        else
        {
            _mainPageNavigationService.NavigateTo<ITaskPage>();
        }

        return Task.CompletedTask;
    }
}
