using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts;

public interface ITaskItemRepository
{
    bool AddReminderToTask(TaskItem task, Reminder reminder);
    TaskItem AddTask(TaskItem task);

    TaskItem AddTagToTask(TaskItem task, TagItem tag);
    TaskItem RemoveTagsFromTask(TaskItem task);
    TaskItem RemoveTagFromTask(TaskItem task, TagItem tag);
    List<TaskItem> GetActiveTasks();
    List<TaskItem> GetActiveTasksFromCategory(int categoryId);
    TaskItem? GetTaskById(int id);
    List<TaskItemVersion> GetTaskItemVersions(int taskId);
    void UpdateTaskListOrders(List<TaskItem> taskItems);
    TaskItem UpdateTaskItem(TaskItem task);
    void DeleteTask(TaskItem task);
    void DeleteTasksInCategory(int categoryId);
    TaskItem RestoreTask(TaskItem task, int newListOrder);
    TaskItemVersion? GetTaskItemVersionById(int versionId);
    TaskItem RestoreTaskToVersion(int taskId, int versionId);
}