using System.Globalization;
using System.Windows.Data;

namespace Modules.Categories.Views.Controls;

public class BoolToChevronConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true)
        {
            return "\uE70D"; // ChevronDown
        }

        return "\uE76C"; // ChevronRight
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
