using Modules.Common.DataModels;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Modules.Common.Views.ValueConverters;

public class TagColorConverter : BaseValueConverter
{
    public ColorType ColorType { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TagColor tagColor)
        {
            return Convert(tagColor);
        }
#if !DEBUG

        throw new ArgumentException($"Cannot convert {value} to TagColor.");
#else
        return new SolidColorBrush(Color.FromRgb(255, 0, 255));
#endif
    }

    public Brush Convert(TagColor tagColor)
    {
        string resourceName = $"Tag{tagColor}";

        if (ColorType == ColorType.Background)
        {
            //resourceName += "Bg";
        }
        else if (ColorType == ColorType.Border)
        {
            //resourceName += "Border";
        }

        var resource = Application.Current.TryFindResource(resourceName);

        ArgumentNullException.ThrowIfNull(resource);

        return new SolidColorBrush((Color)resource);
    }
}