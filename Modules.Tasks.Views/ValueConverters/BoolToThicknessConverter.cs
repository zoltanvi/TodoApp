using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;

namespace Modules.Tasks.Views.ValueConverters;

public class BoolToThicknessConverter : BaseValueConverter
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }

    public double NegativeLeft { get; set; }
    public double NegativeTop { get; set; }
    public double NegativeRight { get; set; }
    public double NegativeBottom { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true 
            ? new Thickness(Left, Top, Right, Bottom) 
            : new Thickness(NegativeLeft, NegativeTop, NegativeRight, NegativeBottom);
    }
}