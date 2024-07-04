using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts;

public interface ITaskItemRepository
{
    bool AddReminderToTask(TaskItem task, Reminder reminder);
    TaskItem AddTask(TaskItem task);
    List<TaskItem> GetActiveTasks();
    List<TaskItem> GetActiveTasksFromCategory(int categoryId);
    TaskItem? GetTaskById(int id);
    void UpdateTaskListOrders(List<TaskItem> taskItems);
}