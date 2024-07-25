using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Prism.Events;

namespace Modules.Categories.Views.Mappings;

public static class CategoryViewModelMappings
{
    public static Category Map(this CategoryItemViewModel vm)
    {
        return new Category
        {
            Id = vm.Id,
            Name = vm.Name,
            ListOrder = vm.ListOrder,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            IsDeleted = vm.IsDeleted,
        };
    }

    public static List<Category> MapList(this IEnumerable<CategoryItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static CategoryItemViewModel MapToViewModel(this Category category, IEventAggregator eventAggregator)
    {
        return new CategoryItemViewModel(eventAggregator)
        {
            Id = category.Id,
            Name = category.Name,
            ListOrder = category.ListOrder,
            CreationDate = category.CreationDate,
            ModificationDate = category.ModificationDate,
            IsDeleted = category.IsDeleted
        };
    }

    public static List<CategoryItemViewModel> MapToViewModelList(
        this IEnumerable<Category> categoryList,
        IEventAggregator eventAggregator) =>
        categoryList.Select(x => x.MapToViewModel(eventAggregator)).ToList();
}
