using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.DataBinding;
using Modules.Common.Events;
using Modules.Common.Services;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Common.Views.Services;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using TodoApp.Themes;
using TodoApp.Time;
using TodoApp.WindowHandling;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace TodoApp;

[AddINotifyPropertyChangedInterface]
public class MainWindowViewModel : BaseViewModel
{
    private int _prevLeft;
    private int _prevTop;
    private int _prevWidth;
    private int _prevHeight;

    private readonly IWindowService _windowService;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUIScaler _uiScaler;
    private readonly ThemeManager _themeManager;
    private readonly IOverlayPageNavigationService _overlayPageNavigationService;
    private readonly TrayIconModule _trayIconModule;
    private double _myWidth;
    private double _myHeight;
    private readonly TimeDisplayService _timeDisplayService;
    private static WindowSettings WindowSettings => AppSettings.Instance.WindowSettings;
    private static ApplicationSettings ApplicationSettings => AppSettings.Instance.ApplicationSettings;
    public MainWindowViewModel(
        IWindowService windowService,
        IMediator mediator,
        IEventAggregator eventAggregator,
        IUIScaler uiScaler,
        ThemeManager themeManager,
        IOverlayPageNavigationService overlayPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(windowService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(themeManager);
        ArgumentNullException.ThrowIfNull(overlayPageNavigationService);
        
        _windowService = windowService;
        _mediator = mediator;
        _eventAggregator = eventAggregator;
        _uiScaler = uiScaler;
        // ThemeManager is injected so it is being created
        _themeManager = themeManager;
        _overlayPageNavigationService = overlayPageNavigationService;

        MinimizeCommand = new RelayCommand(() => _windowService.Minimize());
        MaximizeCommand = new RelayCommand(() => _windowService.Maximize());
        CloseCommand = new RelayCommand(CloseWindow);
        ToggleSideMenuCommand = new RelayCommand(() =>
        {
            if (!overlayPageNavigationService.PageVisible)
            {
                AppSettings.Instance.SessionSettings.SideMenuOpen ^= true;
            }
        });

        _windowService.Deactivated += (s, e) => _windowService.Topmost = ApplicationSettings.AlwaysOnTop;
        _windowService.Resized += (s, e) =>
        {
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(IsMaximized));
            OnPropertyChanged(nameof(IsMaximizedOrDocked));
        };

        _windowService.Loaded += OnWindowLoaded;
        _windowService.Closing += OnWindowClosing;
        //_windowService.Closed += (s, e) => _context.Dispose();
        _windowService.RoundedCornersChanged += (s, e) => OnPropertyChanged(nameof(IsRoundedCornersAllowed));

        ApplicationSettings.SettingsChanged += OnApplicationSettingsChanged;

        _trayIconModule = new TrayIconModule(_windowService)
        {
            IsEnabled = ApplicationSettings.ExitToTray
        };

        _timeDisplayService = new TimeDisplayService(timeLong =>
        {
            CurrentTime = timeLong;
            OnPropertyChanged(nameof(CurrentTime));
        });

        TimerService.Instance.CreateTimer(TimeSpan.FromSeconds(3), UpdateAppSettingsWindowSettings, true);
    }

    /// <summary>
    /// Updates the window size and position values in the WindowSettings if any value have changed
    /// </summary>
    private void UpdateAppSettingsWindowSettings(object? sender, EventArgs e)
    {
        if (_prevLeft != (int)_windowService.Left ||
            _prevTop != (int)_windowService.Top)
        {
            _prevLeft = (int)_windowService.Left;
            _prevTop = (int)_windowService.Top;

            WindowSettings.Left = _prevLeft;
            WindowSettings.Top = _prevTop;
        }

        if (_prevWidth != (int)_windowService.Width ||
            _prevHeight != (int)_windowService.Height)
        {
            _prevWidth = (int)_windowService.Width;
            _prevHeight = (int)_windowService.Height;

            if (!_windowService.IsMinimized)
            {
                // Only save the window size when the window is NOT minimized,
                // because the window size is invalid in that case.
                WindowSettings.Width = _prevWidth;
                WindowSettings.Height = _prevHeight;
            }
        }
    }

    private void OnWindowLoaded(object sender, EventArgs e)
    {
        var left = WindowSettings.Left;
        var top = WindowSettings.Top;
        var width = WindowSettings.Width;
        var height = WindowSettings.Height;

        bool outOfBounds =
            (left <= SystemParameters.VirtualScreenLeft - width) ||
            (top <= SystemParameters.VirtualScreenTop - height) ||
            (SystemParameters.VirtualScreenLeft +
                SystemParameters.VirtualScreenWidth <= left) ||
            (SystemParameters.VirtualScreenTop +
                SystemParameters.VirtualScreenHeight <= top);

        // Check whether the last saved window position is visible on any screen or not
        // Restore the window position and size only if it is visible.
        // Note: If the restored window state would not be visible,
        //       the default window position is at center of screen
        if (!outOfBounds)
        {
            // Restore saved position and size
            _windowService.SetPosition(WindowSettings.Left, WindowSettings.Top);
            _windowService.SetSize(WindowSettings.Width, WindowSettings.Height);
        }
    }

    private void CloseWindow()
    {
        if (_trayIconModule.IsEnabled)
        {
            _trayIconModule.MinimizeToTray();
        }
        else
        {
            _trayIconModule.Dispose();
            _windowService.Close();
        }
    }

    public double WindowMinimumWidth { get; set; } = 220;
    public double WindowMinimumHeight { get; set; } = 200;
    public bool IsMaximized => _windowService.IsMaximized;
    public bool IsMaximizedOrDocked => _windowService.IsMaximizedOrDocked;
    public bool IsRoundedCornersAllowed => _windowService.IsRoundedCornersAllowed && ApplicationSettings.RoundedWindowCorners;

    #region Workaround

    // WORKAROUND properties for MultiBinding bug
    // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
    public double MyWidth
    {
        get => _myWidth;
        set
        {
            if (value.Equals(_myWidth)) return;
            _myWidth = value;
            OnPropertyChanged(nameof(MyWidth));
            OnPropertyChanged(nameof(ClipRect));
            OnPropertyChanged(nameof(OuterClipRect));
        }
    }

    public double MyHeight
    {
        get => _myHeight;
        set
        {
            if (value.Equals(_myHeight)) return;
            _myHeight = value;
            OnPropertyChanged(nameof(MyHeight));
            OnPropertyChanged(nameof(ClipRect));
            OnPropertyChanged(nameof(OuterClipRect));
        }
    }

    public int OuterMargin => 2 * AppSettings.Instance.ApplicationSettings.ResizeBorderSize;
    public Rect ClipRect => new(0, 0, MyWidth, MyHeight);
    public Rect OuterClipRect => new(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);

    #endregion Workaround

    public double ContentPadding { get; set; } = 0;

    // The padding of the inner content of the main window
    public Thickness InnerContentPadding => new(ContentPadding);

    // The size of the resize border around the window
    public int ResizeBorder => IsMaximized ? 0 : AppSettings.Instance.ApplicationSettings.ResizeBorderSize;

    // The size of the resize border around the window, taking into account the outer margin
    public Thickness ResizeBorderThickness => new(ResizeBorder);


    public bool SaveIconVisible { get; set; }
    public long CurrentTime { get; set; }

    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand ToggleSideMenuCommand { get; }


    private void OnApplicationSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ApplicationSettings.ExitToTray))
        {
            _trayIconModule.IsEnabled = ApplicationSettings.ExitToTray;
        }
        else if (e.PropertyName == nameof(ApplicationSettings.RoundedWindowCorners))
        {
            OnPropertyChanged(nameof(IsRoundedCornersAllowed));
        }
    }

    public void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {
                _uiScaler.ZoomIn();
            }
            else if (e.Delta < 0)
            {
                _uiScaler.ZoomOut();
            }
        }
    }

    // Global hotkeys for the window
    public void OnKeyDown(object sender, KeyEventArgs e)
    {
        Key key = e.Key;

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            switch (key)
            {
                case Key.F:
                {
                    _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Publish();
                    break;
                }
                case Key.Z:
                {
                    //IoC.UndoManager.Undo();
                    break;
                }
                case Key.Y:
                {
                    //IoC.UndoManager.Redo();
                    break;
                }
                case Key.Subtract:
                {
                    // Ctrl + -
                    _uiScaler.ZoomOut();
                    break;
                }
                case Key.Add:
                {
                    // Ctrl + +
                    _uiScaler.ZoomIn();
                    break;
                }
                case Key.N:
                {
                    // Ctrl + Space
                    // Set focus on task page bottom text editor
                    _eventAggregator.GetEvent<HotkeyPressedCtrlSpaceEvent>().Publish();
                    break;
                }
            }
        }
    }

    private void OnWindowClosing(object? sender, EventArgs e)
    {
        WindowSettings.Left = (int)_windowService.Left;
        WindowSettings.Top = (int)_windowService.Top;

        if (!_windowService.IsMinimized)
        {
            // Only save the window size when the window is NOT minimized,
            // because the window size is invalid in that case.
            WindowSettings.Width = (int)_windowService.Width;
            WindowSettings.Height = (int)_windowService.Height;
        }

        _mediator.Publish(new ApplicationClosingEvent());
    }
}
