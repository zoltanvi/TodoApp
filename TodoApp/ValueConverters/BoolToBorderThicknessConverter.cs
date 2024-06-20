using Modules.Common.Views.ValueConverters;
using Modules.Settings.Contracts.ViewModels;
using System.Globalization;
using System.Windows;

namespace TodoApp.ValueConverters;

public class BoolToBorderThicknessConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // true: maximized, false: docked
        var isMaximized = (bool)value;

        return new Thickness(
            isMaximized 
                ? 0 
                : AppSettings.Instance.AppWindowSettings.ResizeBorderSize);
    }
}
