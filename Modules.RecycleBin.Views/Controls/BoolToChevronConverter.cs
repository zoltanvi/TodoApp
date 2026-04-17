using System.Globalization;
using System.Windows.Data;

namespace Modules.RecycleBin.Views.Controls;

public class BoolToChevronConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true
            ? "\uE70D"  // ChevronDown  (open)
            : "\uE76C"; // ChevronRight (closed)
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
