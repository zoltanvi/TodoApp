using Modules.Common.DataModels;

namespace Modules.Tasks.Views.ValueConverters.Extensions;

internal static class ThicknessExtensions
{
    public static double ConvertToWidth(this Thickness thickness) => thickness switch
    {
        Thickness.VeryThin => 1,
        Thickness.Thin => 3,
        Thickness.Medium => 5,
        Thickness.Thick => 8,
        Thickness.ExtraThick => 15,
        Thickness.ExtremelyThick => 20,
        _ => throw new ArgumentOutOfRangeException(nameof(thickness), "No such thickness.")
    };
}
