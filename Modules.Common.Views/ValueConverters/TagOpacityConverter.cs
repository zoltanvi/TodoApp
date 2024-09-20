using System.Globalization;

namespace Modules.Common.Views.ValueConverters;

public class TagOpacityConverter : BaseValueConverter
{
    public override object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is bool isDarkMode && isDarkMode)
        {
            return 0.24;
        }

        return 0.1;
    }
}
