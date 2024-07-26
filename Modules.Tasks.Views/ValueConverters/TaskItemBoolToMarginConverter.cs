using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.Views.ValueConverters;

public class TaskItemBoolToMarginConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            return new System.Windows.Thickness(0, 22, 0, 0);
        }

        return new System.Windows.Thickness(0);
    }
}
