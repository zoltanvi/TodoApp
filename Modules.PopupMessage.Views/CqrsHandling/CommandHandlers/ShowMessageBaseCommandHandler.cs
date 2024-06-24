using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views.CqrsHandling.CommandHandlers;

public class ShowMessageBaseCommandHandler
{
    protected Task Handle(ShowMessageBaseCommand request, MessageType messageType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Message);

        PopupMessageManager.Instance.ShowMessage(request.Message, messageType, request.Duration);

        return Task.CompletedTask;
    }

}