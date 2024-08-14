namespace Modules.Tasks.Contracts.Models;

public class TaskItemVersion
{
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public required string Content { get; set; }
    public required bool IsContentPlainText { get; set; }
    public required string ContentPreview { get; set; }
    public DateTime VersionDate { get; set; }

    // Navigation property to refer back to the TaskItem
    public TaskItem TaskItem { get; set; }
}
