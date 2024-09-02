using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.Views.ValueConverters;

public class MoveToCategoryNameConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return stringValue.Replace("_", "__");
        }

        return string.Empty;
    }
}
