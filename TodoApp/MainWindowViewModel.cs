using System.ComponentModel;
using MediatR;
using Modules.Common.Cqrs.Events;
using Modules.Common.DataBinding;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;
using System.Windows;
using System.Windows.Input;
using Modules.Common.ViewModel;
using TodoApp.Themes;
using TodoApp.WindowHandling;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace TodoApp;

public class MainWindowViewModel : BaseViewModel
{
    private readonly IWindowService _windowService;
    private readonly IMediator _mediator;
    private readonly IUIScaler _uiScaler;
    private readonly ThemeManager _themeManager;
    private readonly TrayIconModule _trayIconModule;
    private static WindowSettings WindowSettings => AppSettings.Instance.WindowSettings;
    private static AppWindowSettings AppWindowSettings => AppSettings.Instance.AppWindowSettings;
    public MainWindowViewModel(
        IWindowService windowService,
        IMediator mediator,
        IUIScaler uiScaler,
        ThemeManager themeManager)
    {
        ArgumentNullException.ThrowIfNull(windowService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(themeManager);

        _windowService = windowService;
        _mediator = mediator;
        _uiScaler = uiScaler;
        // ThemeManager is injected so it is being created
        _themeManager = themeManager;

        MinimizeCommand = new RelayCommand(() => _windowService.Minimize());
        MaximizeCommand = new RelayCommand(() => _windowService.Maximize());
        CloseCommand = new RelayCommand(CloseWindow);
        ToggleSideMenuCommand = new RelayCommand(() => AppSettings.Instance.ThemeSettings.DarkMode ^= true);

        _windowService.Deactivated += (s, e) => _windowService.Topmost = AppWindowSettings.AlwaysOnTop;
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

        AppWindowSettings.PropertyChanged += OnAppWindowSettingsChanged;

        _trayIconModule = new TrayIconModule(_windowService)
        {
            IsEnabled = AppWindowSettings.ExitToTray
        };
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

    // AppWindowSettings.RoundedWindowCorners and this property both must be true for the rounded corners to work
    public bool IsRoundedCornersAllowed => _windowService.IsRoundedCornersAllowed;

    #region Workaround
    // WORKAROUND properties for MultiBinding bug
    // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
    public double MyWidth { get; set; }
    public double MyHeight { get; set; }
    public int OuterMargin => 2 * AppSettings.Instance.AppWindowSettings.ResizeBorderSize;
    public Rect ClipRect => new(0, 0, MyWidth, MyHeight);
    public Rect OuterClipRect => new(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);

    #endregion Workaround

    public double ContentPadding { get; set; } = 0;

    // The padding of the inner content of the main window
    public Thickness InnerContentPadding => new(ContentPadding);

    // The size of the resize border around the window
    public int ResizeBorder => IsMaximized ? 0 : AppSettings.Instance.AppWindowSettings.ResizeBorderSize;

    // The size of the resize border around the window, taking into account the outer margin
    public Thickness ResizeBorderThickness => new(ResizeBorder);


    public bool SaveIconVisible { get; set; }
    public long CurrentTime { get; set; }

    public bool MessageLineVisible { get; set; }

    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand ToggleSideMenuCommand { get; }


    private void OnAppWindowSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppWindowSettings.ExitToTray))
        {
            _trayIconModule.IsEnabled = AppWindowSettings.ExitToTray;
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

        //if (key == Key.Escape) IoC.OneEditorOpenService.EditMode(null);

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            // Ctrl + Z, Ctrl + Y
            if (key == Key.Z)
            {
                //IoC.UndoManager.Undo();
            }
            else if (key == Key.Y)
            {
                //IoC.UndoManager.Redo();
            }
            else if (key == Key.Subtract)
            {
                _uiScaler.ZoomOut();
            }
            else if (key == Key.Add)
            {
                _uiScaler.ZoomIn();
            }
            else if (key == Key.E)
            {
                // Ctrl + E
                // Set focus on task page bottom text editor
                MediatorOBSOLETE.NotifyClients(ViewModelMessages.FocusBottomTextEditor);
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                // Ctrl + Shift + J, Ctrl + Shift + L
                if (key == Key.J || key == Key.L)
                {
                    // TODO: think about it
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
