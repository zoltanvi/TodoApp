using System.Windows;
using System.Windows.Resources;

namespace TodoApp.WindowHandling;

public sealed class TrayIconModule : IDisposable
{
    private IWindowService _windowService;
    private readonly NotifyIcon _notifyIcon;
    private bool _enabled;
    private bool _visible;
    private ContextMenuStrip _contextMenuStrip;
    private WindowState _previousWindowState;

    public bool IsEnabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            _notifyIcon.Visible = _enabled;
        }
    }

    public TrayIconModule(IWindowService windowService)
    {
        ArgumentNullException.ThrowIfNull(windowService);

        _windowService = windowService;
        _previousWindowState = _windowService.WindowState;

        StreamResourceInfo? imageInfo;
        try
        {
            var path = "pack://application:,,,/TodoApp;component/Resources/Images/Tray.ico";
            var iconUri = new Uri(path);
            imageInfo = System.Windows.Application.GetResourceStream(iconUri);
            ArgumentNullException.ThrowIfNull(imageInfo);
        }
        catch (Exception e)
        {
            throw new Exception("Could not load Tray icon!", e);
        }

        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon(imageInfo.Stream),
            Visible = false,
            Text = "TodoApp"
        };

        _notifyIcon.MouseClick += OnNotifyIconMouseClick;

        CreateContextMenu();
    }

    private void OnNotifyIconMouseClick(object? sender, MouseEventArgs e)
    {
        if (IsEnabled && e.Button == MouseButtons.Left)
        {
            if (_visible && _windowService.WindowState != WindowState.Minimized)
            {
                MinimizeToTray();
            }
            else
            {
                ShowWindow();
            }
        }
    }

    public void MinimizeToTray()
    {
        if (IsEnabled)
        {
            _previousWindowState = _windowService.WindowState;
            _windowService.Hide();
            _visible = false;
        }
    }

    public void ShowWindow()
    {
        if (!_visible)
        {
            _windowService.Show();
            _visible = true;
        }

        _windowService.WindowState = _previousWindowState;
    }

    private void CreateContextMenu()
    {
        _contextMenuStrip = new ContextMenuStrip();

        ToolStripMenuItem[] menuItems =
        [
            new("Show", null, OnShowClicked),
            new("Exit", null, OnExitClicked)
        ];

        _contextMenuStrip.Items.AddRange(menuItems);

        _notifyIcon.ContextMenuStrip = _contextMenuStrip;
    }

    private void OnShowClicked(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Dispose();
        _windowService.Close();
    }

    public void Dispose()
    {
        _notifyIcon.Dispose();
        _contextMenuStrip.Dispose();
    }
}
