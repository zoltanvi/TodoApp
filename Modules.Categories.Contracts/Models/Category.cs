namespace Modules.Categories.Contracts.Models;

public class Category
{
    public int Id { get; set; }
    public int? ParentCategoryId { get; set; }
    public required string Name { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted => DeletedDate.HasValue;
}
