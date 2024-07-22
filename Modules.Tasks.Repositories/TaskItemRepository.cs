﻿using Microsoft.EntityFrameworkCore;
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

        dbTask.Tags.Add(tag);

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

    public TaskItem? GetTaskById(int id) => _context.Tasks.Find(id);

    public List<TaskItemVersion> GetTaskItemVersions(int taskId)
    {
        var dbTask = _context.Tasks.Find(taskId);
        ArgumentNullException.ThrowIfNull(dbTask);

        return _context.TaskItemVersions
            .Where(x => x.TaskId == taskId)
            .ToList();
    }

    public bool AddReminderToTask(TaskItem task, Reminder reminder)
    {
        if (task == null) return false;

        var dbTask = _context.Tasks.Find(task.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        var dbReminder = _context.Reminders.Find(reminder.Id);

        if (dbReminder != null)
        {
            throw new InvalidOperationException("A reminder with this ID already exists.");
        }

        reminder.TaskId = task.Id;
        reminder.TaskItem = dbTask;

        dbTask.Reminders.Add(reminder);
        _context.SaveChanges();

        return true;
    }

    public List<TaskItem> GetActiveTasks()
    {
        return _context.Tasks
            .Where(x => !x.IsDeleted)
            .Include(x => x.Reminders)
            .Include(x => x.Versions)
            .Include(x => x.Tags)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public List<TaskItem> GetActiveTasksFromCategory(int categoryId)
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

    public void UpdateTaskListOrders(List<TaskItem> taskItems)
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
        var dbTask = _context.Tasks.Find(task.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.ListOrder = task.ListOrder;
        dbTask.Pinned = task.Pinned;
        dbTask.IsDone = task.IsDone;
        dbTask.MarkerColor = task.MarkerColor;
        dbTask.BorderColor = task.BorderColor;
        dbTask.BackgroundColor = task.BackgroundColor;

        if (!dbTask.Content.Equals(task.Content))
        {
            var oldVersion = new TaskItemVersion
            {
                TaskId = dbTask.Id,
                Content = dbTask.Content,
                ContentPreview = dbTask.ContentPreview,
                VersionDate = dbTask.ModificationDate
            };

            _context.TaskItemVersions.Add(oldVersion);
        }

        dbTask.Content = task.Content;
        dbTask.ContentPreview = task.ContentPreview;
        dbTask.ModificationDate = DateTime.Now;

        _context.SaveChanges();

        return dbTask;
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
}
