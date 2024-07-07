using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;

namespace Modules.Tasks.Views.ValueConverters;

public class BoolToVisibilityAndConverter : BaseMultiValueConverter<BoolToVisibilityAndConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is [bool firstBool, bool secondBool])
        {
            return firstBool && secondBool 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }
}