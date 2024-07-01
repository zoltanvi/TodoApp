using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;

namespace Modules.Tasks.TextEditor.ValueConverters;

internal class BoolToVisibilityOrConverter : BaseMultiValueConverter<BoolToVisibilityOrConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return Visibility.Collapsed;
        }

        if (values[0] is bool firstBool && 
            values[1] is bool secondBool)
        {
            return (firstBool) || (secondBool) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }
}