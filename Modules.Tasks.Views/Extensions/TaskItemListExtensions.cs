using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls.TaskItemView;

namespace Modules.Tasks.Views.Extensions;

public static class TaskItemListExtensions
{
    public static void SetListOrdersToIndex(this List<TaskItem>? taskItems)
    {
        if (taskItems == null) return;

        for (var i = 0; i < taskItems.Count; i++)
        {
            taskItems[i].ListOrder = i;
        }
    }

    public static void SetListOrdersToIndex(this IEnumerable<TaskItem>? taskItems)
    {
        if (taskItems == null) return;

        int i = 0;
        foreach (var taskItem in taskItems)
        {
            taskItem.ListOrder = i;
            i++;
        }
    }

    public static void SetListOrdersToIndex(this IEnumerable<TaskItemViewModel>? taskItems)
    {
        if (taskItems == null) return;

        int i = 0;
        foreach (var taskItem in taskItems)
        {
            taskItem.ListOrder = i;
            i++;
        }
    }
}
