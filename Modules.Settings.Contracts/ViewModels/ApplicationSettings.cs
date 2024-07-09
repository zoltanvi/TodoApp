using Modules.Common.DataModels;

namespace Modules.Settings.Contracts.ViewModels;

public class ApplicationSettings : SettingsBase
{
    public bool AlwaysOnTop { get; set; }
    public bool AutoStart { get; set; }
    public bool RoundedWindowCorners { get; set; } = true;
    public bool ExitToTray { get; set; }
    public bool CloseSideMenuOnPageChange { get; set; }
#if DEBUG
    = false;
    #else
    = true;
#endif

    public string AppBorderColor { get; set; } = "#BDBDBD";

    public double WindowMinimumWidth { get; set; } = 220;
    public double WindowMinimumHeight { get; set; } = 200;
    public int ResizeBorderSize { get; set; } = 9;
    public TitleBarHeight TitleBarHeight { get; set; } = TitleBarHeight.Normal;

    public const double TurnedOnRoundedCornersRadius = 8;
}
