using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.Views.ValueConverters;

public class HorizontalAlignmentConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HorizontalAlignment alignment)
        {
            return alignment switch
            {
                HorizontalAlignment.Left => System.Windows.HorizontalAlignment.Left,
                HorizontalAlignment.Center => System.Windows.HorizontalAlignment.Center,
                HorizontalAlignment.Right => System.Windows.HorizontalAlignment.Right,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return System.Windows.HorizontalAlignment.Left;
    }
}