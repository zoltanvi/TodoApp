using Modules.Settings.Contracts;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace TodoApp.Themes;

public class ThemeEditorService : IThemeEditorService
{
    public void SetThemeColor(string resourceName, string value)
    {
        var currentResources = Application.Current.Resources;

        if (currentResources.Contains(resourceName))
        {
            currentResources[resourceName] = new SolidColorBrush(HexToColor(value));
        }
    }

    public string GetThemeColor(string resourceName)
    {
        var brush = Application.Current.Resources[resourceName] as SolidColorBrush;
        return brush?.Color.ToString() ?? "#FF00FF";
    }

    private static Color HexToColor(string value) =>
        (Color)ColorConverter.ConvertFromString(value);
}