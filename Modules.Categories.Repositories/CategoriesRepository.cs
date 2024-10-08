﻿using Modules.Categories.Contracts;
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

    public Category? GetCategoryByName(string name) =>
        _context.Categories.FirstOrDefault(c => c.Name.ToUpper() == name.ToUpper());

    public List<Category> GetActiveCategories()
    {
        return _context.Categories
            .Where(x => !x.IsDeleted)
            .Where(x => x.Id != Constants.RecycleBinCategoryId)
            .OrderBy(x => x.ListOrder)
            .ToList();
    }

    public Category UpdateCategory(Category category)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.Name = category.Name;
        dbCategory.ListOrder = category.ListOrder;
        dbCategory.ModificationDate = DateTime.Now;

        _context.SaveChanges();

        return dbCategory;
    }

    public void DeleteCategory(Category category)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.IsDeleted = true;
        dbCategory.ListOrder = -1;

        _context.SaveChanges();
    }

    public Category RestoreCategory(Category category, int newListOrder)
    {
        var dbCategory = _context.Categories.Find(category.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        dbCategory.IsDeleted = false;
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

    public int GetActiveCategoriesCount()
    {
        return _context.Categories
            .Where(x => !x.IsDeleted)
            .Count(x => x.Id != Constants.RecycleBinCategoryId);
    }
}
