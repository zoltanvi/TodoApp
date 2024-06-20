using Modules.Common.Views.ValueConverters;

namespace TodoApp.ValueConverters;

public class AppBorderBrushConverter : DefaultBorderBrushConverter
{
    protected override string DefaultResourceName { get; } = "SurfaceDim";
}
