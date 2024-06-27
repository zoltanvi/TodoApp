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

    public TaskItem? GetTaskById(int id) => _context.Tasks.Find(id);

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
            .OrderBy(x => x.ListOrder)
            .ToList();
    }
}
