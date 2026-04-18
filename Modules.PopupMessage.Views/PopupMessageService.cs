using Modules.PopupMessage.Contracts;
using Modules.PopupMessage.Views.Models;

namespace Modules.PopupMessage.Views;

public class PopupMessageService : IPopupMessageService
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(1.5);

    public void ShowInfo(string message, TimeSpan? duration = null) =>
        Show(message, MessageType.Info, duration);

    public void ShowWarning(string message, TimeSpan? duration = null) =>
        Show(message, MessageType.Warning, duration);

    public void ShowError(string message, TimeSpan? duration = null) =>
        Show(message, MessageType.Error, duration);

    private static void Show(string message, MessageType messageType, TimeSpan? duration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        PopupMessageManager.Instance.ShowMessage(message, messageType, duration ?? DefaultDuration);
    }
}
