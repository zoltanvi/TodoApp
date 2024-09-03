using GongSolutions.Wpf.DragDrop;

namespace Modules.Common.Views.DragDrop;

public class CustomDropHandler : DefaultDropHandler
{
    public static CustomDropHandler Instance { get; } = new();

    public override void Drop(IDropInfo dropInfo) => DragDropHelper.SimpleDrop(dropInfo);
}