namespace Modules.PopupMessage.Contracts;

public interface IPopupMessageService
{
    void ShowInfo(string message, TimeSpan? duration = null);

    void ShowWarning(string message, TimeSpan? duration = null);

    void ShowError(string message, TimeSpan? duration = null);
}
