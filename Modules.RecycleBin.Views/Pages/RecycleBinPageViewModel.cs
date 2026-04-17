using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Events;
using Modules.Categories.Contracts.Models;
using Modules.Common.Events;
using Modules.Common.ViewModel;
using Modules.Common.Views.Controls;
using Modules.RecycleBin.Repositories;
using Modules.RecycleBin.Views.Controls;
using Modules.RecycleBin.Views.Mappings;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Modules.RecycleBin.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class RecycleBinPageViewModel : BaseViewModel
{
    private const int CollapseGroupsItemLimit = 10;
    private const int GroupTotalContentLengthLimit = CollapseGroupsItemLimit * 200;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;
    private readonly ICategoriesRepository _categoryRepository;
    private readonly RecycleBinRepository _recycleBinRepository;
    
    public RecycleBinPageViewModel(
        IMediator mediator,
        IEventAggregator eventAggregator,
        ICategoriesRepository categoryRepository,
        RecycleBinRepository recycleBinRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(categoryRepository);
        ArgumentNullException.ThrowIfNull(recycleBinRepository);

        _mediator = mediator;
        _eventAggregator = eventAggregator;
        _categoryRepository = categoryRepository;
        _recycleBinRepository = recycleBinRepository;

        SearchBoxViewModel = new SearchBoxViewModel();
        InitializeGroupItems();

        SubscribeToEvents();
    }

    public ObservableCollection<RecycleBinGroupItemViewModel> GroupItems { get; } = new();
    public ICollectionView GroupItemsView { get; set; }

    public bool IsEmpty => GroupItems.Count == 0;

    public SearchBoxViewModel SearchBoxViewModel { get; set; }

    private void SubscribeToEvents()
    {
        _eventAggregator.GetEvent<CategoryDeletedEvent>().Subscribe(OnCategoryDeleted);
        _eventAggregator.GetEvent<TaskRestoredEvent>().Subscribe(OnTaskRestored);
        _eventAggregator.GetEvent<AllTasksInCategoryRestoredEvent>().Subscribe(OnAllTasksInCategoryRestored);
        _eventAggregator.GetEvent<CategoryTreeRestoredEvent>().Subscribe(OnCategoryTreeRestored);
        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Subscribe(OnCtrlFPressed);

        SearchBoxViewModel.SearchTermsChanged += OnSearchTermsChanged;
    }

    private void UnsubscribeFromEvents()
    {
        _eventAggregator.GetEvent<TaskRestoredEvent>().Unsubscribe(OnTaskRestored);
        _eventAggregator.GetEvent<CategoryDeletedEvent>().Unsubscribe(OnCategoryDeleted);
        _eventAggregator.GetEvent<AllTasksInCategoryRestoredEvent>().Unsubscribe(OnAllTasksInCategoryRestored);
        _eventAggregator.GetEvent<CategoryTreeRestoredEvent>().Unsubscribe(OnCategoryTreeRestored);
        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Unsubscribe(OnCtrlFPressed);

        SearchBoxViewModel.SearchTermsChanged -= OnSearchTermsChanged;
    }

    private void OnTaskRestored(TaskRestoredPayload payload)
    {
        foreach (var group in GroupItems)
        {
            var task = group.FindTaskInTree(payload.TaskId, out var ownerGroup);
            if (task != null && ownerGroup != null)
            {
                ownerGroup.Items.Remove(task);
                RemoveEmptyGroups();
                OnPropertyChanged(nameof(IsEmpty));
                GroupItemsView.Refresh();
                return;
            }
        }
    }

    private void OnAllTasksInCategoryRestored(int categoryId)
    {
        var group = FindGroupInTree(categoryId);
        if (group == null) return;

        RemoveGroupFromTree(categoryId);
        RemoveEmptyGroups();

        OnPropertyChanged(nameof(IsEmpty));
        GroupItemsView.Refresh();
    }

    private void OnCategoryTreeRestored(int rootCategoryId)
    {
        var group = GroupItems.FirstOrDefault(x => x.CategoryId == rootCategoryId);
        if (group != null)
        {
            GroupItems.Remove(group);
        }
        else
        {
            RemoveGroupFromTree(rootCategoryId);
            RemoveEmptyGroups();
        }

        OnPropertyChanged(nameof(IsEmpty));
        GroupItemsView.Refresh();
    }

    private void OnCategoryDeleted(int categoryId)
    {
        RebuildGroupItems();
    }

    private void RebuildGroupItems()
    {
        GroupItems.Clear();
        BuildHierarchicalGroups();

        OnPropertyChanged(nameof(IsEmpty));
        GroupItemsView?.Refresh();
    }

    private void InitializeGroupItems()
    {
        GroupItemsView = CollectionViewSource.GetDefaultView(GroupItems);
        GroupItemsView.Filter = FilterGroupItems;

        BuildHierarchicalGroups();

        OnPropertyChanged(nameof(IsEmpty));
    }

    private void BuildHierarchicalGroups()
    {
        var deletedTasksGroupByCategory = _recycleBinRepository.GetDeletedTasksGroupByCategory();
        var deletedCategories = _categoryRepository.GetDeletedCategories();

        if (deletedTasksGroupByCategory.Count == 0 && deletedCategories.Count == 0) return;

        var tasksByCategoryId = deletedTasksGroupByCategory.ToDictionary(g => g.Key, g => g.ToList());
        var categoriesById = deletedCategories.ToDictionary(c => c.Id);

        var categoryIdsWithTasks = new HashSet<int>(tasksByCategoryId.Keys);
        var categoryIdsWithDeletedTasks = new HashSet<int>(categoryIdsWithTasks);

        // For each category with tasks, find deleted ancestors to build full hierarchy paths
        foreach (var categoryId in categoryIdsWithDeletedTasks)
        {
            CollectDeletedAncestors(categoryId, categoriesById, categoryIdsWithTasks);
        }

        // Find root groups: deleted categories that are either root-level or whose parent is not deleted
        var rootCategoryIds = categoryIdsWithTasks
            .Where(id => categoriesById.ContainsKey(id))
            .Where(id =>
            {
                var cat = categoriesById[id];
                return cat.ParentCategoryId == null ||
                       !categoriesById.ContainsKey(cat.ParentCategoryId.Value) ||
                       !categoryIdsWithTasks.Contains(cat.ParentCategoryId.Value);
            })
            .ToList();

        // Also include categories with tasks that are NOT deleted (tasks were individually deleted)
        var nonDeletedCategoryIdsWithTasks = tasksByCategoryId.Keys
            .Where(id => !categoriesById.ContainsKey(id))
            .ToList();

        foreach (var rootId in rootCategoryIds)
        {
            var group = BuildGroupTree(rootId, categoriesById, tasksByCategoryId, categoryIdsWithTasks);
            if (group != null && group.HasAnyContent())
            {
                GroupItems.Add(group);
            }
        }

        // Add flat groups for tasks in non-deleted categories
        foreach (var categoryId in nonDeletedCategoryIdsWithTasks)
        {
            var items = CreateTaskItemVMs(tasksByCategoryId[categoryId]);
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null) continue;

            int totalLength = GetTotalLength(items);
            bool closeGroup = totalLength > GroupTotalContentLengthLimit;
            var groupIsOpen = !closeGroup && items.Count <= CollapseGroupsItemLimit;

            GroupItems.Add(new RecycleBinGroupItemViewModel(groupIsOpen, items, _mediator)
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
            });
        }
    }

    private RecycleBinGroupItemViewModel? BuildGroupTree(
        int categoryId,
        Dictionary<int, Category> categoriesById,
        Dictionary<int, List<TaskItem>> tasksByCategoryId,
        HashSet<int> relevantCategoryIds)
    {
        if (!categoriesById.TryGetValue(categoryId, out var category)) return null;

        var items = tasksByCategoryId.TryGetValue(categoryId, out var tasks)
            ? CreateTaskItemVMs(tasks)
            : new ObservableCollection<RecycleBinTaskItemViewModel>();

        int totalLength = GetTotalLength(items);
        bool closeGroup = totalLength > GroupTotalContentLengthLimit;
        var groupIsOpen = !closeGroup && items.Count <= CollapseGroupsItemLimit;

        var group = new RecycleBinGroupItemViewModel(groupIsOpen, items, _mediator)
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
        };

        // Find child categories that are relevant (deleted and part of a hierarchy with tasks)
        var childIds = relevantCategoryIds
            .Where(id => categoriesById.ContainsKey(id) && categoriesById[id].ParentCategoryId == categoryId)
            .ToList();

        foreach (var childId in childIds)
        {
            var childGroup = BuildGroupTree(childId, categoriesById, tasksByCategoryId, relevantCategoryIds);
            if (childGroup != null && childGroup.HasAnyContent())
            {
                group.Children.Add(childGroup);
            }
        }

        return group;
    }

    private static void CollectDeletedAncestors(
        int categoryId,
        Dictionary<int, Category> categoriesById,
        HashSet<int> categoryIdsToInclude)
    {
        if (!categoriesById.TryGetValue(categoryId, out var category)) return;

        var parentId = category.ParentCategoryId;
        while (parentId.HasValue && categoriesById.ContainsKey(parentId.Value))
        {
            categoryIdsToInclude.Add(parentId.Value);
            parentId = categoriesById[parentId.Value].ParentCategoryId;
        }
    }

    private ObservableCollection<RecycleBinTaskItemViewModel> CreateTaskItemVMs(List<TaskItem> tasks)
    {
        var items = new ObservableCollection<RecycleBinTaskItemViewModel>();
        foreach (var task in tasks)
        {
            items.Add(task.MapToRecycleBinTaskItem(_mediator));
        }
        return items;
    }

    private RecycleBinGroupItemViewModel? FindGroupInTree(int categoryId)
    {
        foreach (var group in GroupItems)
        {
            var found = group.FindGroupInTree(categoryId);
            if (found != null) return found;
        }
        return null;
    }

    private void RemoveGroupFromTree(int categoryId)
    {
        var topLevel = GroupItems.FirstOrDefault(x => x.CategoryId == categoryId);
        if (topLevel != null)
        {
            GroupItems.Remove(topLevel);
            return;
        }

        foreach (var group in GroupItems)
        {
            if (RemoveGroupFromChildren(group, categoryId)) return;
        }
    }

    private static bool RemoveGroupFromChildren(RecycleBinGroupItemViewModel parent, int categoryId)
    {
        var child = parent.Children.FirstOrDefault(x => x.CategoryId == categoryId);
        if (child != null)
        {
            parent.Children.Remove(child);
            return true;
        }

        foreach (var c in parent.Children)
        {
            if (RemoveGroupFromChildren(c, categoryId)) return true;
        }

        return false;
    }

    private void RemoveEmptyGroups()
    {
        var toRemove = GroupItems.Where(g => !g.HasAnyContent()).ToList();
        foreach (var g in toRemove)
        {
            GroupItems.Remove(g);
        }

        foreach (var group in GroupItems)
        {
            RemoveEmptyChildGroups(group);
        }
    }

    private static void RemoveEmptyChildGroups(RecycleBinGroupItemViewModel parent)
    {
        var toRemove = parent.Children.Where(c => !c.HasAnyContent()).ToList();
        foreach (var c in toRemove)
        {
            parent.Children.Remove(c);
        }

        foreach (var child in parent.Children)
        {
            RemoveEmptyChildGroups(child);
        }
    }

    private bool FilterGroupItems(object obj)
    {
        if (obj is RecycleBinGroupItemViewModel groupItem)
        {
            // Empty, reset sub-search
            if (string.IsNullOrWhiteSpace(SearchBoxViewModel.SearchText))
            {
                groupItem.SetSearchTerms([]);
                return true;
            }

            // Search in category task items
            var hasItems = groupItem.SetSearchTerms(SearchBoxViewModel.SearchTerms);
            return hasItems;
        }

        return false;
    }

    private void OnCtrlFPressed()
    {
        SearchBoxViewModel.IsSearchBoxOpen = true;
        SearchBoxViewModel.TriggerSearchBoxFocus = true;
    }

    private void OnSearchTermsChanged()
    {
        GroupItemsView.Refresh();
    }

    protected override void OnDispose() => UnsubscribeFromEvents();

    private int GetTotalLength(IEnumerable<RecycleBinTaskItemViewModel> items) => 
        items.Sum(item => item.Content.GetContentInPlainText().Length);
}
