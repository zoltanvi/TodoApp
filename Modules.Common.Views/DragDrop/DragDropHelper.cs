using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Collections;
using System.Windows;

namespace Modules.Common.Views.DragDrop;

public static class DragDropHelper
{
    public static void SimpleDrop(IDropInfo dropInfo)
    {
        if (dropInfo?.DragInfo == null)
        {
            return;
        }

        int newIndex = dropInfo.UnfilteredInsertIndex;
        IList sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();

        if (sourceList == null)
        {
            // Fixes the crash where the text is currently edited but somehow the drag & drop is active.
            return;
        }

        object item = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<object>().FirstOrDefault();

        ArgumentNullException.ThrowIfNull(item);

        int oldIndex = sourceList.IndexOf(item);

        // Decrement index because the item is going to be removed before it is inserted again
        newIndex--;

        // Dropped above itself in the list, the decrementing was not necessary.
        if (newIndex + 1 <= oldIndex)
        {
            newIndex++;
        }

        if (oldIndex != -1)
        {
            sourceList.Remove(item);

            if ((dropInfo.DragInfo.VisualSource as FrameworkElement)?.DataContext is IDropIndexModifier dropIndexModifier)
            {
                newIndex = dropIndexModifier.GetModifiedDropIndex(newIndex, item);
            }

            // The list got smaller, check for over-indexing (inserting into last index)
            newIndex = Math.Min(newIndex, sourceList.Count);

            sourceList.Insert(newIndex, item);
        }
    }
}
