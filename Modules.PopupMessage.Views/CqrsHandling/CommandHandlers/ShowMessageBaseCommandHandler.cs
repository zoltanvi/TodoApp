using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views.CqrsHandling.CommandHandlers;

public class ShowMessageBaseCommandHandler
{
    protected Task Handle(ShowMessageBaseCommand request, MessageType messageType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Message);

        var viewModel = PopupMessageViewModel.Instance;
        viewModel.Duration = request.Duration;
        viewModel.Message = request.Message;
        viewModel.MessageType = messageType;
        viewModel.Visible = true;
        return Task.CompletedTask;
    }

}