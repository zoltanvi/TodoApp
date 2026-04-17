using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Prism.Events;
using System.Collections.ObjectModel;

namespace Modules.Categories.Views.Mappings;

public static class CategoryViewModelMappings
{
    public static Category Map(this CategoryItemViewModel vm)
    {
        return new Category
        {
            Id = vm.Id,
            ParentCategoryId = vm.ParentCategoryId,
            Name = vm.Name,
            ListOrder = vm.ListOrder,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            DeletedDate = vm.DeletedDate,
        };
    }

    public static List<Category> MapList(this IEnumerable<CategoryItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static CategoryItemViewModel MapToViewModel(this Category category, IEventAggregator eventAggregator)
    {
        return new CategoryItemViewModel(eventAggregator)
        {
            Id = category.Id,
            ParentCategoryId = category.ParentCategoryId,
            Name = category.Name,
            ListOrder = category.ListOrder,
            CreationDate = category.CreationDate,
            ModificationDate = category.ModificationDate,
            DeletedDate = category.DeletedDate
        };
    }

    public static List<CategoryItemViewModel> MapToViewModelList(
        this IEnumerable<Category> categoryList,
        IEventAggregator eventAggregator) =>
        categoryList.Select(x => x.MapToViewModel(eventAggregator)).ToList();

    public static List<CategoryItemViewModel> BuildTree(
        this List<Category> allCategories,
        IEventAggregator eventAggregator)
    {
        var allVms = allCategories.Select(c => c.MapToViewModel(eventAggregator)).ToList();
        var lookup = allVms.ToLookup(x => x.ParentCategoryId);

        foreach (var vm in allVms)
        {
            var children = lookup[vm.Id].OrderBy(c => c.ListOrder).ToList();
            vm.Children = new ObservableCollection<CategoryItemViewModel>(children);
            vm.HasChildren = children.Count > 0;
        }

        // Root items have no parent
        var roots = lookup[null].OrderBy(x => x.ListOrder).ToList();
        AssignDepth(roots, 0);
        return roots;
    }

    private static void AssignDepth(IEnumerable<CategoryItemViewModel> items, int depth)
    {
        foreach (var item in items)
        {
            item.Depth = depth;
            AssignDepth(item.Children, depth + 1);
        }
    }
}
