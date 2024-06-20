using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Services.CqrsHandling.EventHandlers;

public class ApplicationClosingEventHandler : INotificationHandler<ApplicationClosingEvent>
{
    private readonly IAppSettingsService _appSettingsService;

    public ApplicationClosingEventHandler(IAppSettingsService appSettingsService)
    {
        ArgumentNullException.ThrowIfNull(appSettingsService);
        _appSettingsService = appSettingsService;
    }

    public Task Handle(ApplicationClosingEvent notification, CancellationToken cancellationToken)
    {
        _appSettingsService.UpdateDatabaseFromAppSettings(AppSettings.Instance);
        return Task.CompletedTask;
    }
}
