using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Views.Models;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.PopupMessage.Views;

[AddINotifyPropertyChangedInterface]
public class PopupMessageManager : BaseViewModel
{
    public static PopupMessageManager Instance { get; } = new();

    private PopupMessageManager()
    {
        AppSettings.Instance.ThemeSettings.SettingsChanged += OnThemeSettingsChanged;
        CloseMessageCommand = new RelayCommand(() =>
        {
            // Changing it to true triggers the animation to close, changing back to false does not.
            Closed = true;
            Closed = false;
        });
    }


    private void OnThemeSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeSettings.DarkMode))
        {
            OnPropertyChanged(nameof(MessageType));
        }
    }

    public void ShowMessage(string message, MessageType messageType, TimeSpan duration)
    {
        MessageType = messageType;
        Message = message;

        // Set the duration first for the animation. Adding one second for the slide in and out animations
        MessageDuration = duration + TimeSpan.FromSeconds(1);

        // Changing it to true triggers the animation, changing back to false does not.
        TriggerAnimation = true;
        TriggerAnimation = false;

        // Make sure to trigger ui update
        OnPropertyChanged(nameof(MessageType));
    }

    public MessageType MessageType { get; set; } = MessageType.Invalid;
    public string Message { get; set; } = string.Empty;
    public bool TriggerAnimation { get; set; }
    public bool Closed { get; set; }
    public TimeSpan MessageDuration { get; set; } = TimeSpan.FromSeconds(4);
    public ICommand CloseMessageCommand { get; }
}
