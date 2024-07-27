using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Services.CqrsHandling.EventHandlers;

public class ApplicationOpeningEventHandler : INotificationHandler<ApplicationOpeningEvent>
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly IUIScaler _uiScaler;

    public ApplicationOpeningEventHandler(IAppSettingsService appSettingsService, IUIScaler uiScaler)
    {
        ArgumentNullException.ThrowIfNull(appSettingsService);
        ArgumentNullException.ThrowIfNull(uiScaler);

        _appSettingsService = appSettingsService;
        _uiScaler = uiScaler;
    }

    public Task Handle(ApplicationOpeningEvent notification, CancellationToken cancellationToken)
    {
        _appSettingsService.UpdateAppSettingsFromDatabase(AppSettings.Instance);

        // Set the saved scaling to restore the UI
        _uiScaler.SetScaling(AppSettings.Instance.WindowSettings.Scaling);
        
        return Task.CompletedTask;
    }
}
