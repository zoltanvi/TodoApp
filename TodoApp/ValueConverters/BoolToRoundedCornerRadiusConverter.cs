using Modules.Common.Views.ValueConverters;
using Modules.Settings.Contracts.ViewModels;
using System.Globalization;
using System.Windows;

namespace TodoApp.ValueConverters;

internal class BoolToRoundedCornerRadiusConverter : BaseValueConverter
{
    public bool TopLeft { get; set; }
    public bool TopRight { get; set; }
    public bool BottomLeft { get; set; }
    public bool BottomRight { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool turnedOn && turnedOn)
        {
            var radius = ApplicationSettings.TurnedOnRoundedCornersRadius;

            var topLeft = TopLeft ? radius : 0;
            var topRight = TopRight ? radius : 0;
            var bottomLeft = BottomLeft ? radius : 0;
            var bottomRight = BottomRight ? radius : 0;

            return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
        }

        return new CornerRadius(0);
    }
}