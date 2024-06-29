using System.Globalization;
using System.Windows;

namespace Modules.Common.Views.ValueConverters;

public class BoolToVisibilityConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var bValue = value switch
        {
            bool b => b,
            string s => s.ToLowerInvariant() == "true",
            _ => false
        };

        return (bValue) ? Visibility.Visible : Visibility.Collapsed;
    }
}
