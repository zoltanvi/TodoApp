using Microsoft.EntityFrameworkCore;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly TaskItemDbContext _context;

    public TaskItemRepository(TaskItemDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public TaskItem AddTask(TaskItem task)
    {
        task.CreationDate = DateTime.Now;
        task.ModificationDate = DateTime.Now;

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return task;
    }

    public List<TaskItem> AddTasks(List<TaskItem> tasks)
    {
        foreach (var taskItem in tasks)
        {
            taskItem.CreationDate = DateTime.Now;
            taskItem.ModificationDate = DateTime.Now;
            _context.Tasks.Add(taskItem);
        }

        _context.SaveChanges();

        return tasks;
    }

    public TaskItem AddTagToTask(TaskItem task, TagItem tag)
    {
        var dbTask = _context.Tasks
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == task.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbTag = _context.Tags.Find(tag.Id);
        ArgumentNullException.ThrowIfNull(dbTag);

        if (dbTask.Tags.Contains(tag))
        {
            throw new ArgumentException($"Task is already tagged with {tag.Name}");
        }

        dbTask.Tags.Add(dbTag);

        _context.SaveChanges();

        return dbTask;
    }

    public TaskItem RemoveTagFromTask(TaskItem task, TagItem tag)
    {
        var dbTask = _context.Tasks
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == task.Id);

        ArgumentNullException.ThrowIfNull(dbTask);

        var dbTag = dbTask.Tags.FirstOrDefault(x => x.Id == tag.Id);
        ArgumentNullException.ThrowIfNull(dbTag);

        dbTask.Tags.Remove(dbTag);

        _context.SaveChanges();

        return dbTask;
    }

    public TaskItem RemoveTagsFromTask(TaskItem task)
    {
        var dbTask = _context.Tasks
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == task.Id);

        ArgumentNullException.ThrowIfNull(dbTask);

        if (dbTask.Tags.Count != 0)
        {
            dbTask.Tags.Clear();
        }

        _context.SaveChanges();

        return dbTask;
    }

    public void RemoveTagsFromTasks(IEnumerable<TaskItem> taskList)
    {
        foreach (var taskItem in taskList)
        {
            var dbTask = _context.Tasks
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == taskItem.Id);

            ArgumentNullException.ThrowIfNull(dbTask);

            if (dbTask.Tags.Count != 0)
            {
                dbTask.Tags.Clear();
            }
        }

        _context.SaveChanges();
    }

    public TaskItem? GetTaskById(int id, bool includeNavigation = false)
    {
        if (includeNavigation)
        {
            return _context.Tasks
                .Include(x => x.Reminders)
                .Include(x => x.Versions)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);
        }

        return _context.Tasks.Find(id);
    }

    public List<TaskItemVersion> GetTaskItemVersions(int taskId)
    {
        var dbTask = _context.Tasks.Find(taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        return _context.TaskItemVersions
            .Where(x => x.TaskId == taskId)
            .ToList();
    }

    public List<TaskItem> GetActiveTasksFromCategory(int categoryId, bool includeNavigation = false)
    {
        if (includeNavigation)
        {
            return _context.Tasks
                .Where(x => x.CategoryId == categoryId)
                .Where(x => !x.IsDeleted)
                .Include(x => x.Reminders)
                .Include(x => x.Versions)
                .Include(x => x.Tags)
                .OrderBy(x => x.ListOrder)
                .ToList();
        }

        return _context.Tasks
            .Where(x => x.CategoryId == categoryId)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public List<TaskItem> GetDeletedTasksFromCategory(int categoryId, bool includeNavigation = false)
    {
        if (includeNavigation)
        {
            return _context.Tasks
                .Where(x => x.CategoryId == categoryId)
                .Where(x => x.IsDeleted)
                .Include(x => x.Reminders)
                .Include(x => x.Versions)
                .Include(x => x.Tags)
                .OrderBy(x => x.ListOrder)
                .ToList();
        }

        return _context.Tasks
            .Where(x => x.CategoryId == categoryId)
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public void UpdateTaskListOrders(IEnumerable<TaskItem> taskItems)
    {
        foreach (var updatedTaskItem in taskItems)
        {
            var dbTaskItem = _context.Tasks.Find(updatedTaskItem.Id);
            ArgumentNullException.ThrowIfNull(dbTaskItem);

            dbTaskItem.ListOrder = updatedTaskItem.ListOrder;
        }

        _context.SaveChanges();
    }

    public TaskItem UpdateTaskItem(TaskItem task)
    {
        TaskItem updatedTask = UpdateTaskItemWithoutContextSave(task);

        _context.SaveChanges();

        return updatedTask;
    }

    public void UpdateTaskItems(IEnumerable<TaskItem> taskItems)
    {
        foreach (var task in taskItems)
        {
            UpdateTaskItemWithoutContextSave(task);
        }

        _context.SaveChanges();
    }

    public void MoveTaskToCategory(TaskItem taskItem, int categoryId)
    {
        var dbTask = _context.Tasks.Find(taskItem.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.CategoryId = categoryId;

        _context.SaveChanges();
    }

    public void MoveTasksToCategory(IEnumerable<TaskItem> taskItems, int categoryId)
    {
        foreach (var taskItem in taskItems)
        {
            var dbTask = _context.Tasks.Find(taskItem.Id);
            ArgumentNullException.ThrowIfNull(dbTask);

            dbTask.CategoryId = categoryId;
        }

        _context.SaveChanges();
    }

    public void DeleteTask(TaskItem task)
    {
        var dbTask = _context.Tasks.Find(task.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.DeletedDate = DateTime.Now;
        dbTask.IsDeleted = true;
        dbTask.ListOrder = -1;

        _context.SaveChanges();
    }

    public void DeleteTasks(IEnumerable<TaskItem> itemsToDelete)
    {
        foreach (var taskItem in itemsToDelete)
        {
            var dbTask = _context.Tasks.Find(taskItem.Id);
            ArgumentNullException.ThrowIfNull(dbTask);

            dbTask.DeletedDate = DateTime.Now;
            dbTask.IsDeleted = true;
            dbTask.ListOrder = -1;
        }

        _context.SaveChanges();
    }

    public void DeleteTasksInCategory(int categoryId)
    {
        var dbTasks = _context.Tasks
            .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
            .ToList();

        if (dbTasks.Count == 0) return;

        foreach (TaskItem dbTask in dbTasks)
        {
            dbTask.DeletedDate = DateTime.Now;
            dbTask.IsDeleted = true;
            dbTask.ListOrder = -1;
        }

        _context.SaveChanges();
    }

    public void RestoreTasksInCategory(int categoryId, int startingListOrder)
    {
        var dbTasks = _context.Tasks
            .Where(x => x.CategoryId == categoryId && x.IsDeleted)
            .ToList();

        if (dbTasks.Count == 0) return;

        var listOrder = startingListOrder;

        foreach (TaskItem dbTask in dbTasks)
        {
            dbTask.DeletedDate = null;
            dbTask.IsDeleted = false;
            dbTask.IsDone = false;
            dbTask.Pinned = false;
            dbTask.ListOrder = listOrder;

            listOrder++;
        }

        _context.SaveChanges();
    }

    public TaskItem RestoreTask(TaskItem task, int newListOrder)
    {
        var dbTask = _context.Tasks.Find(task.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.DeletedDate = null;
        dbTask.IsDeleted = false;
        dbTask.IsDone = false;
        dbTask.Pinned = false;
        dbTask.ListOrder = newListOrder;

        _context.SaveChanges();

        return dbTask;
    }

    public TaskItemVersion? GetTaskItemVersionById(int versionId) =>
        _context.TaskItemVersions.FirstOrDefault(x => x.Id == versionId);

    public TaskItem RestoreTaskToVersion(int taskId, int versionId)
    {
        var dbTask = _context.Tasks
            .Include(x => x.Versions)
            .FirstOrDefault(x => x.Id == taskId);

        ArgumentNullException.ThrowIfNull(dbTask);

        var dbVersion = dbTask.Versions.FirstOrDefault(x => x.Id == versionId);
        ArgumentNullException.ThrowIfNull(dbVersion);

        var oldVersion = new TaskItemVersion
        {
            TaskId = dbTask.Id,
            Content = dbTask.Content,
            IsContentPlainText = dbTask.IsContentPlainText,
            ContentPreview = dbTask.ContentPreview,
            VersionDate = dbTask.ModificationDate
        };

        // Remove this because it will be the current content
        _context.TaskItemVersions.Remove(dbVersion);

        // Add this because this was the previous content
        _context.TaskItemVersions.Add(oldVersion);

        dbTask.Content = dbVersion.Content;
        dbTask.ContentPreview = dbVersion.ContentPreview;
        dbTask.IsContentPlainText = dbVersion.IsContentPlainText;
        dbTask.ModificationDate = DateTime.Now;

        _context.SaveChanges();

        return dbTask;
    }

    private TaskItem UpdateTaskItemWithoutContextSave(TaskItem taskItem)
    {
        var dbTask = _context.Tasks.Find(taskItem.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.ListOrder = taskItem.ListOrder;
        dbTask.Pinned = taskItem.Pinned;
        dbTask.IsDone = taskItem.IsDone;
        dbTask.MarkerColor = taskItem.MarkerColor;
        dbTask.BorderColor = taskItem.BorderColor;
        dbTask.BackgroundColor = taskItem.BackgroundColor;

        if (!dbTask.Content.Equals(taskItem.Content))
        {
            var oldVersion = new TaskItemVersion
            {
                TaskId = dbTask.Id,
                Content = dbTask.Content,
                IsContentPlainText = dbTask.IsContentPlainText,
                ContentPreview = dbTask.ContentPreview,
                VersionDate = dbTask.ModificationDate
            };

            _context.TaskItemVersions.Add(oldVersion);
        }

        dbTask.Content = taskItem.Content;
        dbTask.ContentPreview = taskItem.ContentPreview;
        dbTask.IsContentPlainText = taskItem.IsContentPlainText;
        dbTask.ModificationDate = DateTime.Now;

        return dbTask;
    }
}
