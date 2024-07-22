using MediatR;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Tasks.Contracts.Cqrs.Commands;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class OpenTagSelectorCommandHandler : IRequestHandler<OpenTagSelectorCommand>
{
    private readonly IOverlayPageNavigationService _navigationService;

    public OpenTagSelectorCommandHandler(IOverlayPageNavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        
        _navigationService = navigationService;
    }

    public Task Handle(OpenTagSelectorCommand request, CancellationToken cancellationToken)
    {
        _navigationService.NavigateTo<ITagSelectorPage>(request.TaskId);

        return Task.CompletedTask;
    }
}
