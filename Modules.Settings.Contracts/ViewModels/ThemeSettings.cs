using Modules.Common.DataModels;

namespace Modules.Settings.Contracts.ViewModels;

public class ThemeSettings : SettingsBase
{
    public bool DarkMode { get; set; } = true;
    public bool HighContrast { get; set; }

    public MaterialThemeStyle ThemeStyle { get; set; } = MaterialThemeStyle.TonalSpot;
    public string SeedColor { get; set; } = "#1D64DD";
    public string AppBorderColor { get; set; } = "#5C6BC0";
    public Thickness AppBorderThickness { get; set; } = Thickness.Thin;
}
