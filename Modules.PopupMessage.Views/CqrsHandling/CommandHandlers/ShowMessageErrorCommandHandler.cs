using MediatR;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views.CqrsHandling.CommandHandlers;

public class ShowMessageErrorCommandHandler : ShowMessageBaseCommandHandler, IRequestHandler<ShowMessageErrorCommand>
{
    public Task Handle(ShowMessageErrorCommand request, CancellationToken cancellationToken)
    {
        return Handle(request, MessageType.Error);

    }
}