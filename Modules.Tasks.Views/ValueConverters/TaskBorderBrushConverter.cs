using Modules.Common;
using Modules.Common.Views.ValueConverters;

namespace Modules.Tasks.Views.ValueConverters;

public class TaskBorderBrushConverter : DefaultBorderBrushConverter
{
    protected override string DefaultResourceName { get; } = Constants.ResourceNames.OutlineVariant;
}
