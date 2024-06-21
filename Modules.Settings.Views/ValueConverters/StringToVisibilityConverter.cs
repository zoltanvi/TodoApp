using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;

namespace Modules.Settings.Views.ValueConverters;

/// <summary>
/// Makes the shortcuts appear only if the shortcut key is not empty on the shortcut settings page.
/// </summary>
public class StringToVisibilityConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }
}