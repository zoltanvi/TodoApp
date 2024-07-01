using Modules.Common.Views.ValueConverters;
using Modules.Tasks.Views.ValueConverters.Extensions;
using System.Globalization;
using System.Windows;
using Thickness = Modules.Common.DataModels.Thickness;

namespace Modules.Tasks.Views.ValueConverters;

internal class ColorBarGridWidthMultiValueConverter : BaseMultiValueConverter<ColorBarGridWidthMultiValueConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 &&
            values[0] is Thickness thickness &&
            values[1] is double scaleValue)
        {
            double width = thickness.ConvertToWidth();
            return width * scaleValue;
        }

        return new GridLength(0);
    }
}