using Modules.Common.Views.ValueConverters;
using Modules.PopupMessage.Views.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Modules.PopupMessage.Views.ValueConverters;

public class TagColorConverter : BaseValueConverter
{
    public ColorType ColorType { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TagPresetColor tagColor)
        {
            return Convert(tagColor);
        }

        throw new ArgumentException($"Cannot convert {value} to TagColor.");
    }

    public Brush Convert(TagPresetColor tagColor)
    {
        string resourceName = $"Tag{tagColor}";

        if (ColorType == ColorType.Background)
        {
            resourceName += "Bg";
        }
        else if (ColorType == ColorType.Border)
        {
            resourceName += "Border";
        }

        return new SolidColorBrush((Color)Application.Current.TryFindResource(resourceName));
    }
}