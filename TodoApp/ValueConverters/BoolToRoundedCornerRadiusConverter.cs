using Modules.Common.Views.ValueConverters;
using Modules.Settings.Contracts.ViewModels;
using System.Globalization;
using System.Windows;

namespace TodoApp.ValueConverters;

internal class BoolToRoundedCornerRadiusConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool turnedOn && turnedOn)
        {
            return new CornerRadius(AppWindowSettings.TurnedOnRoundedCornersRadius);
        }

        return new CornerRadius(0);
    }
}
