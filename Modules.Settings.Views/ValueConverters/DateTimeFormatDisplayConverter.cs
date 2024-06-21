using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Settings.Views.ValueConverters;

public class DateTimeFormatDisplayConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateTimeFormat)
        {
            return DateTime.Now.ToString(dateTimeFormat);
        }

        return DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }
}

