using MediatR;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views.CqrsHandling.CommandHandlers;

public class ShowMessageInfoCommandHandler : ShowMessageBaseCommandHandler, IRequestHandler<ShowMessageInfoCommand>
{
    public Task Handle(ShowMessageInfoCommand request, CancellationToken cancellationToken)
    {
        return Handle(request, MessageType.Info);
    }
}