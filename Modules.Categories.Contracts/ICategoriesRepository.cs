using Modules.Categories.Contracts.Models;

namespace Modules.Categories.Contracts;

public interface ICategoriesRepository
{
    Category AddCategory(Category category);
    void DeleteCategory(Category category);
    List<Category> GetActiveCategories();
    List<Category> GetRootCategories();
    List<Category> GetChildCategories(int parentCategoryId);
    Category? GetCategoryById(int id);
    Category? GetCategoryByName(string name);
    Category? GetCategoryByName(string name, int? parentCategoryId);
    Category RestoreCategory(Category category, int newListOrder);
    Category UpdateCategory(Category category);
    void UpdateCategoryListOrders(List<Category> categories);
    void MoveCategoryToParent(int categoryId, int? newParentId, int newListOrder);
    List<int> GetDescendantCategoryIds(int categoryId);
    List<Category> GetDeletedCategories();
    List<int> GetDeletedDescendantCategoryIds(int categoryId);
    int GetActiveCategoriesCount();
    Category? GetRecycleBin();
}