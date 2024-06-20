using System.Globalization;
using System.Windows;

namespace Modules.Common.Views.ValueConverters;

public class BoolToVisibilityNegatedConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool bValue = false;
        if (value is bool b)
        {
            bValue = b;
        }
        
        return (bValue) ? Visibility.Collapsed : Visibility.Visible;
    }
}
