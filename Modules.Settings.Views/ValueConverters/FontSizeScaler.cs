using Modules.Common.Views.Services;
using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Settings.Views.ValueConverters;

/// <summary>
/// A helper to scale the font sizes with the scale value.
/// </summary>
public class FontSizeScaler : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double val = ((double)value) * UIScaler.StaticScaleValue;
        return val;
    }
}
