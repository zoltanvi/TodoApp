using Modules.Common.ViewModel;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views;

public class PopupMessage : BaseViewModel
{
    public MessageType MessageType { get; set; } = MessageType.Invalid;
    public required string Message { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(4);

}
