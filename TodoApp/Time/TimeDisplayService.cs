using System.Windows.Threading;

namespace TodoApp.Time;

public class TimeDisplayService
{
    private DateTime _initialTime;
    private bool _timeInitialized;
    private readonly DispatcherTimer _timer;
    private readonly Action<long> _setTime;

    public TimeDisplayService(Action<long> setTime)
    {
        ArgumentNullException.ThrowIfNull(setTime);
        _setTime = setTime;

        _timer = new DispatcherTimer(DispatcherPriority.Send)
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };

        _setTime(DateTime.Now.Ticks);

        _timer.Tick += TimerOnTickInitializer;
        _timer.Start();
    }

    // This method sync to the second with the clock
    private void TimerOnTickInitializer(object? sender, EventArgs e)
    {
        if (!_timeInitialized)
        {
            _timeInitialized = true;
            _initialTime = DateTime.Now;
        }
        else
        {
            if (_initialTime.Second != DateTime.Now.Second)
            {
                // Accuracy set to 10 ms. Now switch to tick every second.
                _timer.Tick -= TimerOnTickInitializer;
                _setTime(DateTime.Now.Ticks);

                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += TimerOnTick;
            }
        }
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _setTime(DateTime.Now.Ticks);
    }
}
