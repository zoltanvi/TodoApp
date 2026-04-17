using System.Globalization;
using System.Windows;

namespace Modules.Common.Views.ValueConverters;

public class DepthToMarginConverter : BaseValueConverter
{
    public double IndentSize { get; set; } = 16;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int depth)
        {
            return new Thickness(depth * IndentSize, 0, 0, 0);
        }

        return new Thickness(0);
    }
}
