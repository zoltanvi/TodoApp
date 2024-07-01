using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.TextEditor.ValueConverters;

public class BoolNegatedConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue == false;
        }

        throw new ArgumentException("value is not a bool!");
    }
}
