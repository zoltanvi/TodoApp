using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;

namespace Modules.Tasks.Views.ValueConverters;

internal class TaskCheckboxMarginConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double margin = (double)value;
        return new Thickness(margin, 0, margin, 0);
    }
}