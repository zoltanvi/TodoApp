using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace TodoApp.ValueConverters;

public class AppBorderThicknessConverter : BaseValueConverter
{
    public double Offset { get; set; }
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var uniformLength = 0.4 + Offset;

        if (value is Thickness thickness)
        {
            uniformLength = thickness switch
            {
                Thickness.VeryThin => 0.2 + Offset,
                Thickness.Thin => 0.4 + Offset,
                Thickness.Medium => 1 + Offset,
                Thickness.Thick => 1.5 + Offset,
                Thickness.ExtraThick => 2.5 + Offset,
                Thickness.ExtremelyThick => 3 + Offset,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return new System.Windows.Thickness(uniformLength);
    }
}