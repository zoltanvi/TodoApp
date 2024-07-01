using Modules.Common.Views.ValueConverters;
using System.Globalization;
using Modules.Tasks.Views.ValueConverters.Extensions;
using Thickness = Modules.Common.DataModels.Thickness;

namespace Modules.Tasks.Views.ValueConverters;

public class ColorBarWidthMultiValueConverter : BaseMultiValueConverter<ColorBarWidthMultiValueConverter>
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

        return 0d;
    }
}
