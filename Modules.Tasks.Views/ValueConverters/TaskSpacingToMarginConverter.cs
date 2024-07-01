using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System.Globalization;

namespace Modules.Tasks.Views.ValueConverters;

internal class TaskSpacingToMarginConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double marginThickness = 0;

        if (value is TaskSpacing taskSpacing)
        {
            marginThickness = GetThickness(taskSpacing);
        }

        return new System.Windows.Thickness(0, marginThickness, 0, marginThickness);
    }

    private static double GetThickness(TaskSpacing spacing) => spacing switch
    {
        TaskSpacing.Compact => 2,
        TaskSpacing.Normal => 6,
        TaskSpacing.Comfortable => 10,
        TaskSpacing.Spacious => 20,
        _ => throw new ArgumentOutOfRangeException(nameof(spacing), spacing, "Not defined spacing!")
    };
}