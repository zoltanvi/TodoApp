using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Modules.Common.Views.ValueConverters;

public abstract class DefaultBorderBrushConverter : BaseValueConverter
{
    protected abstract string DefaultResourceName { get; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string brushName)
        {
            if (brushName == Constants.ColorName.Transparent)
            {
                return (SolidColorBrush)Application.Current.TryFindResource(DefaultResourceName);
            }

            return StringRGBToBrushConverter.Instance.Convert(value, targetType, parameter, culture);
        }

        return null;
    }
}
