using Modules.Tasks.Views.Controls.TaskItemView;

namespace Modules.Tasks.Views.Extensions;

public static class TaskItemViewModelListExtensions
{
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
