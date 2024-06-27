namespace Modules.Tasks.Contracts.Models;

public class Reminder
{
    public int Id { get; set; }
    public required DateTime ReminderDate { get; set; }
    public bool IsActive { get; set; }

    // Foreign key property to associate with TaskItem
    public int TaskId { get; set; }

    // Navigation property to refer back to the TaskItem
    public TaskItem? TaskItem { get; set; }
}