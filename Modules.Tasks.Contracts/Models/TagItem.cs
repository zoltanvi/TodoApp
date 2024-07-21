namespace Modules.Tasks.Contracts.Models;

public class TagItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Color { get; set; }

    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
