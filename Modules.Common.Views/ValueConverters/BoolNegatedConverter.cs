using System.Globalization;

namespace Modules.Common.Views.ValueConverters;

/// <summary>
/// Converts bool to its negation (e.g. disable control when source flag is true).
/// </summary>
public class BoolNegatedConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = value switch
        {
            bool x => x,
            _ => false
        };
        return !b;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = value switch
        {
            bool x => x,
            _ => false
        };
        return !b;
    }
}
