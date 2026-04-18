using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;
using Modules.Common;

namespace Modules.Categories.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly CategoryDbContext _context;

    public CategoriesRepository(CategoryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public Category AddCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        category.ModificationDate = DateTime.Now;

        _context.Categories.Add(category);
        _context.SaveChanges();

        return category;
    }

    public Category? GetCategoryById(int id) => _context.Categories.Find(id);

    public Category? GetCategoryByName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _context.Categories.FirstOrDefault(c => c.Name.ToUpper() == name.ToUpper());
    }

    public Category? GetCategoryByName(string name, int? parentCategoryId)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _context.Categories.FirstOrDefault(c =>
            c.Name.ToUpper() == name.ToUpper() &&
            c.ParentCategoryId == parentCategoryId);
    }

    public bool ActiveCategoryExistsWithName(string name, int? parentCategoryId)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _context.Categories.Any(c =>
            c.DeletedDate == null &&
            c.Name.ToUpper() == name.ToUpper() &&
            c.ParentCategoryId == parentCategoryId);
    }

    public List<Category> GetActiveCategories()
    {
        return _context.Categories
            .Where(x => x.DeletedDate == null)
            .Where(x => x.Id != Constants.RecycleBinCategoryId)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public List<Category> GetRootCategories()
    {
        return _context.Categories
            .Where(x => x.DeletedDate == null)
            .Where(x => x.Id != Constants.RecycleBinCategoryId)
            .Where(x => x.ParentCategoryId == null)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public List<Category> GetChildCategories(int parentCategoryId)
    {
        return _context.Categories
            .Where(x => x.DeletedDate == null)
            .Where(x => x.ParentCategoryId == parentCategoryId)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public Category UpdateCategory(Category category)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.Name = category.Name;
        dbCategory.ListOrder = category.ListOrder;
        dbCategory.ParentCategoryId = category.ParentCategoryId;
        dbCategory.ModificationDate = DateTime.Now;

        _context.SaveChanges();

        return dbCategory;
    }

    public void DeleteCategory(Category category)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.DeletedDate = DateTime.Now;
        dbCategory.ListOrder = -1;

        // Cascade soft-delete to children
        var children = _context.Categories
            .Where(x => x.ParentCategoryId == dbCategory.Id && x.DeletedDate == null)
            .ToList();

        foreach (var child in children)
        {
            child.DeletedDate = DateTime.Now;
            child.ListOrder = -1;
            CascadeDeleteChildren(child.Id);
        }

        _context.SaveChanges();
    }

    private void CascadeDeleteChildren(int parentId)
    {
        var children = _context.Categories
            .Where(x => x.ParentCategoryId == parentId && x.DeletedDate == null)
            .ToList();

        foreach (var child in children)
        {
            child.DeletedDate = DateTime.Now;
            child.ListOrder = -1;
            CascadeDeleteChildren(child.Id);
        }
    }

    public Category RestoreCategory(Category category, int newListOrder)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.DeletedDate = null;
        dbCategory.ListOrder = newListOrder;

        _context.SaveChanges();

        return dbCategory;
    }

    public void UpdateCategoryListOrders(List<Category> categories)
    {
        foreach (var updatedCategory in categories)
        {
            var dbCategory = _context.Categories.Find(updatedCategory.Id);
            ArgumentNullException.ThrowIfNull(dbCategory);

            dbCategory.ListOrder = updatedCategory.ListOrder;
        }

        _context.SaveChanges();
    }

    public void MoveCategoryToParent(int categoryId, int? newParentId, int newListOrder)
    {
        var dbCategory = _context.Categories.Find(categoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.ParentCategoryId = newParentId;
        dbCategory.ListOrder = newListOrder;
        dbCategory.ModificationDate = DateTime.Now;

        _context.SaveChanges();
    }

    public List<int> GetDescendantCategoryIds(int categoryId)
    {
        var result = new List<int>();
        CollectDescendants(categoryId, result);
        return result;
    }

    private void CollectDescendants(int parentId, List<int> result)
    {
        var childIds = _context.Categories
            .Where(x => x.ParentCategoryId == parentId && x.DeletedDate == null)
            .Select(x => x.Id)
            .ToList();

        foreach (var childId in childIds)
        {
            result.Add(childId);
            CollectDescendants(childId, result);
        }
    }

    public List<Category> GetDeletedCategories()
    {
        return _context.Categories
            .Where(x => x.DeletedDate != null)
            .Where(x => x.Id != Constants.RecycleBinCategoryId)
            .ToList();
    }

    public List<int> GetDeletedDescendantCategoryIds(int categoryId)
    {
        var result = new List<int>();
        CollectDeletedDescendants(categoryId, result);
        return result;
    }

    private void CollectDeletedDescendants(int parentId, List<int> result)
    {
        var childIds = _context.Categories
            .Where(x => x.ParentCategoryId == parentId && x.DeletedDate != null)
            .Select(x => x.Id)
            .ToList();

        foreach (var childId in childIds)
        {
            result.Add(childId);
            CollectDeletedDescendants(childId, result);
        }
    }

    public int GetActiveCategoriesCount()
    {
        return _context.Categories
            .Where(x => x.DeletedDate == null)
            .Count(x => x.Id != Constants.RecycleBinCategoryId);
    }

    public Category? GetRecycleBin() => _context.Categories.Find(Constants.RecycleBinCategoryId);
}
