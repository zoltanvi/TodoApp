using MediatR;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views.CqrsHandling.CommandHandlers;

public class ShowMessageWarningCommandHandler : ShowMessageBaseCommandHandler, IRequestHandler<ShowMessageWarningCommand>
{
    public Task Handle(ShowMessageWarningCommand request, CancellationToken cancellationToken)
    {
        return Handle(request, MessageType.Warning);
    }
}