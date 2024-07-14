using Modules.Common;

namespace Modules.Tasks.Contracts.Models;

public class TaskItem
{
    public int Id { get; set; }
    public required int CategoryId { get; set; }
    public required string Content { get; set; } // Keep current content here
    public required string ContentPreview { get; set; }
    public int ListOrder { get; set; }
    public bool Pinned { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string MarkerColor { get; set; } = Constants.ColorName.Transparent;
    public string BorderColor { get; set; } = Constants.ColorName.Transparent;
    public string BackgroundColor { get; set; } = Constants.ColorName.Transparent;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    // Navigation properties
    public ICollection<TaskItemVersion> Versions { get; set; } = new List<TaskItemVersion>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
