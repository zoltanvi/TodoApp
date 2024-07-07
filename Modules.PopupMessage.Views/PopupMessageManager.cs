using Modules.Common.ViewModel;
using Modules.Common.Views.Services;
using Modules.PopupMessage.Views.Models;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;

namespace Modules.PopupMessage.Views;

[AddINotifyPropertyChangedInterface]
public class PopupMessageManager : BaseViewModel
{
    private readonly Guid _timer;
    private bool _visible;
    private const int AnimationMilliseconds = 1500;
    private readonly Queue<PopupMessage> _messageQueue;
    private int _currentMessageDuration;

    public static PopupMessageManager Instance { get; } = new();

    private PopupMessageManager()
    {
        _timer = TimerService.Instance.CreateTimer(OnMessageDurationEnded);
        _messageQueue = new Queue<PopupMessage>();
        AppSettings.Instance.ThemeSettings.SettingsChanged += OnThemeSettingsChanged;
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
        _messageQueue.Enqueue(new PopupMessage
        {
            Message = message,
            MessageType = messageType,
            Duration = duration
        });

        ProcessNextMessage();
    }

    public MessageType MessageType { get; set; } = MessageType.Invalid;

    public string Message { get; set; } = string.Empty;

    public bool Visible
    {
        get => _visible;
        set
        {
            var wasVisible = _visible;

            _visible = value;

            if (_visible)
            {
                // Add the animation time to the display time if the message line is not visible yet
                var duration = wasVisible
                    ? _currentMessageDuration
                    : _currentMessageDuration + AnimationMilliseconds;

                TimerService.Instance.StopTimer(_timer);
                TimerService.Instance.ModifyTimerInterval(_timer, duration);
                TimerService.Instance.RestartTimer(_timer);
            }
        }
    }

    private void ProcessNextMessage()
    {
        // Uncomment this prevent interrupting the currently displayed message
        if (/*!Visible && */_messageQueue.Count != 0)
        {
            var messageItem = _messageQueue.Dequeue();

            MessageType = messageItem.MessageType;
            Message = messageItem.Message;
            _currentMessageDuration = (int)messageItem.Duration.TotalMilliseconds;

            Visible = true;

            // Make sure to trigger ui update
            OnPropertyChanged(nameof(MessageType));
        }
    }

    private void OnMessageDurationEnded(object? sender, EventArgs e)
    {
        TimerService.Instance.StopTimer(_timer);
        Visible = false;

        ProcessNextMessage();
    }
}
