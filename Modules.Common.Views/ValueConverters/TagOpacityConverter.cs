using System.Globalization;

namespace Modules.Common.Views.ValueConverters;

public class TagOpacityConverter : BaseMultiValueConverter<TagOpacityConverter>
{
    public override object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values.Length == 2 && values.Cast<bool>().Any(x => x == true))
        {
            return 0.24;
        }

        return 0.1;
    }
}
