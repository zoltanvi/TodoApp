using System.Globalization;
using System.Windows;

namespace Modules.Common.Views.ValueConverters;

public abstract class BoolToVisibilityBaseConverter : BaseValueConverter
{
    protected abstract Visibility TrueValue { get; }
    protected abstract Visibility FalseValue { get; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var bValue = value switch
        {
            bool b => b,
            string s => s.ToLowerInvariant() == "true",
            _ => false
        };

        return (bValue) ? TrueValue : FalseValue;
    }
}

public class BoolToVisibilityConverter : BoolToVisibilityBaseConverter
{
    protected override Visibility TrueValue => Visibility.Visible;
    protected override Visibility FalseValue => Visibility.Collapsed;
}

public class BoolToVisibilityNegatedConverter : BoolToVisibilityBaseConverter
{
    protected override Visibility TrueValue => Visibility.Collapsed;
    protected override Visibility FalseValue => Visibility.Visible;
}


public class BoolToVisibilityHiddenConverter : BoolToVisibilityBaseConverter
{
    protected override Visibility TrueValue => Visibility.Visible;
    protected override Visibility FalseValue => Visibility.Hidden;
}