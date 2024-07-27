using MediatR;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Tasks.Contracts.Cqrs.Commands;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class OpenHistoryCommandHandler : IRequestHandler<OpenHistoryCommand>
{
    private readonly IOverlayPageNavigationService _navigationService;

    public OpenHistoryCommandHandler(IOverlayPageNavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        
        _navigationService = navigationService;
    }

    public Task Handle(OpenHistoryCommand request, CancellationToken cancellationToken)
    {
        _navigationService.NavigateTo<ITaskHistoryPage>(request.TaskId);

        return Task.CompletedTask;
    }
}
