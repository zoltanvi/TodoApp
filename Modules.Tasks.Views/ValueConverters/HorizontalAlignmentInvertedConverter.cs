using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.Views.ValueConverters;

public class HorizontalAlignmentInvertedConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HorizontalAlignment alignment)
        {
            return alignment switch
            {
                HorizontalAlignment.Left => System.Windows.HorizontalAlignment.Right,
                HorizontalAlignment.Center => System.Windows.HorizontalAlignment.Right,
                HorizontalAlignment.Right => System.Windows.HorizontalAlignment.Left,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return System.Windows.HorizontalAlignment.Left;
    }
}