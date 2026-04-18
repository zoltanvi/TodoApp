using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Modules.Categories.Views.Mappings;
using Modules.Common;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;
using System.Collections.ObjectModel;

namespace Modules.Categories.Views.Pages;

/// <summary>
/// Owns the category tree structure, flattened visible list, and expand/collapse persistence.
/// </summary>
public sealed class CategoryPageTreeState
{
    private List<CategoryItemViewModel> _treeRoots = [];
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly Action _onFlatListRebuilt;

    public CategoryPageTreeState(
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator,
        Action onFlatListRebuilt)
    {
        _categoriesRepository = categoriesRepository;
        _eventAggregator = eventAggregator;
        _onFlatListRebuilt = onFlatListRebuilt;
    }

    public ObservableCollection<CategoryItemViewModel> FlattenedItems { get; } = new();

    public void Initialize()
    {
        var activeCategories = _categoriesRepository.GetActiveCategories();
        _treeRoots = activeCategories.BuildTree(_eventAggregator);
        RestoreExpandedStates();
        RebuildFlatList();
    }

    public void ReloadTree(int activeCategoryId)
    {
        var activeCategories = _categoriesRepository.GetActiveCategories();
        _treeRoots = activeCategories.BuildTree(_eventAggregator);
        RestoreExpandedStates();
        EnsureAncestorsExpanded(FindInTree(activeCategoryId)?.ParentCategoryId);
        RebuildFlatList();
    }

    public IEnumerable<CategoryItemViewModel> GetAllCategoriesFlat()
    {
        var result = new List<CategoryItemViewModel>();
        CollectAll(_treeRoots, result);
        return result;
    }

    public CategoryItemViewModel? FindInTree(int categoryId) =>
        FindInTreeRecursive(_treeRoots, categoryId);

    public CategoryItemViewModel? FindParentOf(int categoryId) =>
        FindParentRecursive(_treeRoots, categoryId);

    public void RemoveFromTree(int categoryId)
    {
        var parent = FindParentOf(categoryId);
        if (parent != null)
        {
            var child = parent.Children.FirstOrDefault(c => c.Id == categoryId);
            if (child != null)
            {
                parent.Children.Remove(child);
                parent.HasChildren = parent.Children.Count > 0;
            }
        }
        else
        {
            var root = _treeRoots.FirstOrDefault(c => c.Id == categoryId);
            if (root != null)
            {
                _treeRoots.Remove(root);
            }
        }
    }

    public void EnsureAncestorsExpanded(int? parentCategoryId)
    {
        if (parentCategoryId == null) return;
        var parent = FindInTree(parentCategoryId.Value);
        if (parent == null) return;

        if (parent.ParentCategoryId != null)
        {
            EnsureAncestorsExpanded(parent.ParentCategoryId);
        }
        parent.IsExpanded = true;
    }

    public void SaveExpandedStates()
    {
        var expandedIds = GetAllCategoriesFlat()
            .Where(c => c.IsExpanded && c.HasChildren)
            .Select(c => c.Id);

        AppSettings.Instance.SessionSettings.SetExpandedCategoryIds(expandedIds);
    }

    public void RebuildFlatList()
    {
        FlattenedItems.Clear();
        FlattenVisible(_treeRoots, FlattenedItems);
        _onFlatListRebuilt();
    }

    public void ExpandCategory(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null || !category.HasChildren || category.IsExpanded) return;

        category.IsExpanded = true;
        SaveExpandedStates();
        RebuildFlatList();
    }

    public void CollapseCategory(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null || !category.IsExpanded) return;

        category.IsExpanded = false;
        SaveExpandedStates();
        RebuildFlatList();
    }

    public void ToggleExpand(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null) return;

        category.IsExpanded = !category.IsExpanded;
        SaveExpandedStates();
        RebuildFlatList();
    }

    public void AddNewCategoryViewModel(CategoryItemViewModel vm, int? parentCategoryId)
    {
        vm.Depth = parentCategoryId.HasValue ? (FindInTree(parentCategoryId.Value)?.Depth ?? 0) + 1 : 0;

        if (parentCategoryId.HasValue)
        {
            var parentVm = FindInTree(parentCategoryId.Value);
            if (parentVm != null)
            {
                parentVm.Children.Add(vm);
                parentVm.HasChildren = true;
                parentVm.IsExpanded = true;
                SaveExpandedStates();
            }
        }
        else
        {
            _treeRoots.Add(vm);
        }

        RebuildFlatList();
    }

    public void ApplyRestoredCategory(Category dbCategory, CategoryItemViewModel vm)
    {
        if (dbCategory.ParentCategoryId.HasValue)
        {
            var parentVm = FindInTree(dbCategory.ParentCategoryId.Value);
            if (parentVm != null)
            {
                vm.Depth = parentVm.Depth + 1;
                parentVm.Children.Add(vm);
                parentVm.HasChildren = true;
            }
            else
            {
                vm.ParentCategoryId = null;
                vm.Depth = 0;
                _treeRoots.Add(vm);
            }
        }
        else
        {
            vm.Depth = 0;
            _treeRoots.Add(vm);
        }

        RebuildFlatList();
    }

    private void RestoreExpandedStates()
    {
        var expandedIds = AppSettings.Instance.SessionSettings.GetExpandedCategoryIds();
        if (expandedIds.Count == 0) return;

        foreach (var category in GetAllCategoriesFlat())
        {
            if (expandedIds.Contains(category.Id) && category.HasChildren)
            {
                category.IsExpanded = true;
            }
        }
    }

    private static void CollectAll(IEnumerable<CategoryItemViewModel> items, List<CategoryItemViewModel> result)
    {
        foreach (var item in items)
        {
            result.Add(item);
            CollectAll(item.Children, result);
        }
    }

    private static void FlattenVisible(
        IEnumerable<CategoryItemViewModel> items,
        ObservableCollection<CategoryItemViewModel> target)
    {
        foreach (var item in items)
        {
            target.Add(item);
            if (item.IsExpanded && item.Children.Count > 0)
            {
                FlattenVisible(item.Children, target);
            }
        }
    }

    private static CategoryItemViewModel? FindInTreeRecursive(
        IEnumerable<CategoryItemViewModel> items, int categoryId)
    {
        foreach (var item in items)
        {
            if (item.Id == categoryId) return item;
            var found = FindInTreeRecursive(item.Children, categoryId);
            if (found != null) return found;
        }
        return null;
    }

    private static CategoryItemViewModel? FindParentRecursive(
        IEnumerable<CategoryItemViewModel> items, int categoryId)
    {
        foreach (var item in items)
        {
            if (item.Children.Any(c => c.Id == categoryId)) return item;
            var found = FindParentRecursive(item.Children, categoryId);
            if (found != null) return found;
        }
        return null;
    }
}
