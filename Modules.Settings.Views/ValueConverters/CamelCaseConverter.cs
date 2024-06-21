using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Modules.Settings.Views.ValueConverters;

public partial class CamelCaseConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        var stringValue = value.ToString();

        if (!string.IsNullOrEmpty(stringValue))
        {
            var res = string.Join(" ", MyRegex().Split(stringValue));
            return res;
        }

        throw new ArgumentException("Value cannot be empty");
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    [GeneratedRegex("(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])")]
    private static partial Regex MyRegex();
}
