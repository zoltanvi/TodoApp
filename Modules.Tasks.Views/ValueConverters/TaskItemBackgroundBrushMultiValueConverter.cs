using Modules.Common;
using Modules.Common.Views.ValueConverters;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Modules.Tasks.Views.ValueConverters;

public class TaskItemBackgroundBrushMultiValueConverter : BaseMultiValueConverter<TaskItemBackgroundBrushMultiValueConverter>
{
    private static readonly SolidColorBrush Transparent = new(Color.FromArgb(0, 0, 0, 0));
    private readonly VisualBrush _hatchBrush;
    private readonly Path _path;

    public TaskItemBackgroundBrushMultiValueConverter()
    {
        _path = new Path
        {
            Data = Geometry.Parse("M 0 8 L 8 0"),
            Stroke = (Brush)Application.Current.TryFindResource(Constants.BrushName.Surface3),
            StrokeEndLineCap = PenLineCap.Square,
        };

        _hatchBrush = new VisualBrush
        {
            TileMode = TileMode.Tile,
            Viewbox = new Rect(0, 0, 8, 8),
            ViewboxUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, 8, 8),
            ViewportUnits = BrushMappingMode.Absolute,
            Visual = _path
        };
    }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is bool isDone && 
            values[1] is bool isBackgroundVisible)
        {
            // The task list item background is hatched when the task is done and the background is enabled in the settings
            // The brush is always looked up from resources because this way it can dynamically change during runtime
            if (!isBackgroundVisible) return Transparent;

            if (isDone)
            {
                _path.Stroke = (Brush)Application.Current.TryFindResource(Constants.BrushName.Surface3);
                return _hatchBrush;
            }
            else
            {
                return Application.Current.TryFindResource(Constants.BrushName.TaskBgBrush);
            }
        }

        return new SolidColorBrush(Colors.Magenta);
    }
}
