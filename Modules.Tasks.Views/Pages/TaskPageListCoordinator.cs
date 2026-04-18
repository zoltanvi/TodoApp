using Modules.Common.Extensions;
using Modules.Tasks.Contracts;
using Modules.Tasks.Services.Extensions;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls.TaskItemView;
using Modules.Tasks.Views.Mappings;
using System.Collections.ObjectModel;

namespace Modules.Tasks.Views.Pages;

/// <summary>
/// Owns the task list collection, list-order maintenance, and state-based ordering.
/// </summary>
public sealed class TaskPageListCoordinator
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskPageListCoordinator(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
        Items = new ObservableCollection<TaskItemViewModel>();
    }

    public ObservableCollection<TaskItemViewModel> Items { get; }

    /// <summary>
    /// When true, <see cref="Items"/> changes do not trigger list-order fixes.
    /// </summary>
    public bool IgnoreCollectionChange { get; set; }

    public void SetFirstItem()
    {
        var firstItem = Items.FirstOrDefault();
        if (firstItem != null)
        {
            firstItem.IsFirstItem = true;
        }
    }

    public void MoveTaskItem(int newIndex, TaskItemViewModel taskItem)
    {
        Items.Remove(taskItem);
        Items.Insert(newIndex, taskItem);
    }

    public void FixItemsListOrders(bool persist = false)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
            Items[i].IsFirstItem = i == 0;
        }

        if (persist)
        {
            _taskItemRepository.UpdateTaskListOrders(Items.MapList());
        }
    }

    public List<TaskItem> OrderTasksByState(List<TaskItem> tasks)
    {
        List<TaskItem> orderedTasks = new List<TaskItem>();

        var pinnedItems = tasks.Where(x => x.Pinned);
        var activeItems = tasks.Where(x => !x.IsDone && !x.Pinned);
        var doneItems = tasks.Where(x => x.IsDone);

        orderedTasks.AddRange(pinnedItems);
        orderedTasks.AddRange(activeItems);
        orderedTasks.AddRange(doneItems);

        orderedTasks.SetListOrdersToIndex();

        _taskItemRepository.UpdateTaskListOrders(orderedTasks);

        return orderedTasks;
    }
}
