using System.Globalization;

namespace Modules.Common.Views.ValueConverters;

public class CurrentDateTimeFormatMultiConverter : BaseMultiValueConverter<CurrentDateTimeFormatMultiConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

        if (values.Length == 2 && values[1] is string dateFormat)
        {
            if (values[0] is long ticks)
            {
                return new DateTime(ticks).ToString(dateFormat);
            }

            if (values[0] is DateTime dateTime)
            {
                return dateTime.ToString(dateFormat);
            }
        }

        return "INVALID CurrentDateTimeFormatMultiConverter";
    }
}
