using System.Globalization;
using System.Windows.Media;

namespace Modules.Common.Views.ValueConverters;

/// <summary>
/// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
/// </summary>
public class StringRGBToBrushConverter : BaseValueConverter
{
    private readonly BrushConverter _brushConverter = new();

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                colorString = Constants.ColorName.Transparent;
            }

            // Remove the leading # character
            colorString = colorString.TrimStart('#');

            // Prefixes the input string with a # character, except if it is "Transparent"
            var inputColor = (colorString == Constants.ColorName.Transparent ? string.Empty : "#") + colorString;
            return (SolidColorBrush)_brushConverter.ConvertFrom(inputColor);
        }

        return null;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.ToString();
        }
        else if (value is Color color)
        {
            return color.ToString();
        }

        return null;
    }
}