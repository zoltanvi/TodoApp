using Modules.Common.Views.ValueConverters;
using Modules.Settings.Contracts.ViewModels;
using System.Globalization;

namespace TodoApp.ValueConverters;

internal class BoolToRoundedCornerRadiusDoubleConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool turnedOn && turnedOn)
        {
            return AppWindowSettings.TurnedOnRoundedCornersRadius;
        }

        return 0;
    }
}


