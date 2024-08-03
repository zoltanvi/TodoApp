using Modules.Categories.Contracts.Models;

namespace Modules.Categories.Contracts;

public interface ICategoriesRepository
{
    Category AddCategory(Category category);
    void DeleteCategory(Category category);
    List<Category> GetActiveCategories();
    Category? GetCategoryById(int id);
    Category? GetCategoryByName(string name);
    Category RestoreCategory(Category category, int newListOrder);
    Category UpdateCategory(Category category);
    void UpdateCategoryListOrders(List<Category> categories);
    int GetActiveCategoriesCount();
}