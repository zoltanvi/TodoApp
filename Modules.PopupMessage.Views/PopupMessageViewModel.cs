using Modules.Common.ViewModel;
using Modules.Common.Views.Services;
using Modules.PopupMessage.Views.Models;
using PropertyChanged;

namespace Modules.PopupMessage.Views;

[AddINotifyPropertyChangedInterface]
public class PopupMessageViewModel : BaseViewModel
{
    private readonly Guid _timer;
    private bool _visible;
    private const int AnimationMilliseconds = 1500;

    public static PopupMessageViewModel Instance { get; } = new();

    // TODO: create message queue - display all messages from the queue
    private PopupMessageViewModel()
    {
        _timer = TimerService.Instance.CreateTimer(TickEventHandler);
    }

    private void TickEventHandler(object? sender, EventArgs e)
    {
        TimerService.Instance.StopTimer(_timer);
        Visible = false;
    }

    public MessageType MessageType { get; set; } = MessageType.Invalid;

    public string Message { get; set; }

    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value) return;

            _visible = value;
            if (_visible)
            {
                // Add the slide in and out animation time to the requested display time
                var duration = Duration.Milliseconds + AnimationMilliseconds;

                TimerService.Instance.ModifyTimerInterval(_timer, duration);
                TimerService.Instance.RestartTimer(_timer);
            }

            OnPropertyChanged(nameof(Visible));
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageType));
        }
    }

    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(4);
}
