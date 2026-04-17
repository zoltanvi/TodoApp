namespace Modules.Categories.Contracts.Models;

public class CategoryInfo
{
    public required int Id { get; set; }
    public int? ParentCategoryId { get; set; }
    public required string Name { get; set; }
}