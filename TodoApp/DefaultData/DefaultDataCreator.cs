using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;
using Modules.Common;

namespace TodoApp.DefaultData;

public class DefaultDataCreator
{
    private readonly ICategoriesDbInfoRepository _categoriesDbInfoRepository;
    private readonly ICategoriesRepository _categoriesRepository;

    public DefaultDataCreator(
        ICategoriesDbInfoRepository categoriesDbInfoRepository,
        ICategoriesRepository categoriesRepository)
    {
        ArgumentNullException.ThrowIfNull(categoriesDbInfoRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);

        _categoriesDbInfoRepository = categoriesDbInfoRepository;
        _categoriesRepository = categoriesRepository;
    }

    public void CreateDefaultsIfNeeded()
    {
        CreateDefaultCategory();
    }

    private void CreateDefaultCategory()
    {
        if (!_categoriesDbInfoRepository.Initialized)
        {
            _categoriesDbInfoRepository.Initialize();

            _categoriesRepository.AddCategory(new Category { Name = Constants.CategoryName.Today });

            _categoriesRepository.AddCategory(new Category
            {
                Id = Constants.RecycleBinCategoryId,
                Name = Constants.CategoryName.RecycleBin,
                ListOrder = int.MinValue,
            });

        }
    }
}
