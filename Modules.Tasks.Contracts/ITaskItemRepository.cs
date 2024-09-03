using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts;

public interface ITaskItemRepository
{
    TaskItem AddTask(TaskItem task);
    List<TaskItem> AddTasks(List<TaskItem> tasks);
    TaskItem AddTagToTask(TaskItem task, TagItem tag);
    TaskItem RemoveTagFromTask(TaskItem task, TagItem tag);
    TaskItem RemoveTagsFromTask(TaskItem task);
    void RemoveTagsFromTasks(IEnumerable<TaskItem> taskList);
    List<TaskItem> GetActiveTasksFromCategory(int categoryId, bool includeNavigation = false);
    List<TaskItem> GetDeletedTasksFromCategory(int categoryId, bool includeNavigation = false);
    TaskItem? GetTaskById(int id, bool includeNavigation = false);
    List<TaskItemVersion> GetTaskItemVersions(int taskId);
    void UpdateTaskListOrders(List<TaskItem> taskItems);
    TaskItem UpdateTaskItem(TaskItem task);
    void UpdateTaskItems(IEnumerable<TaskItem> taskItems);
    void MoveTaskToCategory(TaskItem taskItem, int categoryId);
    void MoveTasksToCategory(IEnumerable<TaskItem> taskItems, int categoryId);
    void DeleteTask(TaskItem task);
    void DeleteTasks(IEnumerable<TaskItem> itemsToDelete);
    void DeleteTasksInCategory(int categoryId);
    void RestoreTasksInCategory(int categoryId, int startingListOrder);
    TaskItem RestoreTask(TaskItem task, int newListOrder);
    TaskItemVersion? GetTaskItemVersionById(int versionId);
    TaskItem RestoreTaskToVersion(int taskId, int versionId);
}