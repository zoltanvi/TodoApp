using Modules.Common.DataModels;
using Modules.MaterialTheme;
using Modules.Settings.Contracts.ViewModels;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace TodoApp.Themes;

public class MaterialThemeManagerService
{
    private ThemeSettings ThemeSettings => AppSettings.Instance.ThemeSettings;
    private uint SeedColor { get; set; }
    public Scheme<string> CurrentScheme { get; set; }

    public void UpdateTheme()
    {
        SeedColor = MaterialColorHelper.HexToDecimal(ThemeSettings.SeedColor);
        var corePalette = CorePalette.Of(SeedColor, MapThemeStyle(ThemeSettings.ThemeStyle));

        if (ThemeSettings.DarkMode)
        {
            var darkScheme = new DarkSchemeMapper().Map(corePalette);
            CurrentScheme = darkScheme.Convert(MaterialColorHelper.DecimalToHex);
        }
        else
        {
            var lightScheme = new LightSchemeMapper().Map(corePalette);
            CurrentScheme = lightScheme.Convert(MaterialColorHelper.DecimalToHex);
        }

        UpdateResources();
    }

    private void UpdateResources()
    {
        var currentResources = Application.Current.Resources;

        foreach (var item in CurrentScheme.Enumerate())
        {
            if (currentResources.Contains(item.Key))
            {
                currentResources[item.Key] = new SolidColorBrush(HexToColor(item.Value));
            }
        }
    }

    private static Color HexToColor(string value) =>
        (Color)ColorConverter.ConvertFromString(value);

    private static ThemeStyle MapThemeStyle(MaterialThemeStyle style) => style switch
    {
        MaterialThemeStyle.Spritz => ThemeStyle.Spritz,
        MaterialThemeStyle.TonalSpot => ThemeStyle.TonalSpot,
        MaterialThemeStyle.Vibrant => ThemeStyle.Vibrant,
        MaterialThemeStyle.Expressive => ThemeStyle.Expressive,
        MaterialThemeStyle.Rainbow => ThemeStyle.Rainbow,
        MaterialThemeStyle.FruitSalad => ThemeStyle.FruitSalad,
        MaterialThemeStyle.Content => ThemeStyle.Content,
        MaterialThemeStyle.Monochromatic => ThemeStyle.Monochromatic,
        MaterialThemeStyle.Clock => ThemeStyle.Clock,
        MaterialThemeStyle.ClockVibrant => ThemeStyle.ClockVibrant,
        MaterialThemeStyle.SingleColor => ThemeStyle.SingleColor,
        _ => throw new ArgumentOutOfRangeException(nameof(style))
    };
}