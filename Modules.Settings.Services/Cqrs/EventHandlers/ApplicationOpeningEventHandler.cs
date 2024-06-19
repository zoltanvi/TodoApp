using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Settings.Services.Cqrs.EventHandlers;

public class ApplicationOpeningEventHandler : INotificationHandler<ApplicationOpeningEvent>
{
    private readonly IAppSettingsService _appSettingsService;

    public ApplicationOpeningEventHandler(IAppSettingsService appSettingsService)
    {
        ArgumentNullException.ThrowIfNull(appSettingsService);
        _appSettingsService = appSettingsService;
    }

    public Task Handle(ApplicationOpeningEvent notification, CancellationToken cancellationToken)
    {
        _appSettingsService.UpdateAppSettingsFromDatabase(AppSettings.Instance);
        return Task.CompletedTask;
    }
}
