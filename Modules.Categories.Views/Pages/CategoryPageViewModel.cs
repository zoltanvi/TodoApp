using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Categories.Contracts.Events;
using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Modules.Categories.Views.Events;
using Modules.Categories.Views.Mappings;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Categories.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class CategoryPageViewModel : BaseViewModel
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;
    private readonly IOverlayPageNavigationService _overlayPageNavigationService;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    private List<CategoryItemViewModel> _treeRoots = [];

    public CategoryPageViewModel(
        ICategoriesRepository categoriesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IOverlayPageNavigationService overlayPageNavigationService,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigationService);
        ArgumentNullException.ThrowIfNull(overlayPageNavigationService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _categoriesRepository = categoriesRepository;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;
        _overlayPageNavigationService = overlayPageNavigationService;
        _mediator = mediator;
        _eventAggregator = eventAggregator;

        AddCategoryCommand = new RelayCommand(AddCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        ActiveCategoryId = AppSettings.Instance.SessionSettings.ActiveCategoryId;

        var activeCategories = categoriesRepository.GetActiveCategories();
        _treeRoots = activeCategories.BuildTree(_eventAggregator);
        FlattenedItems = new ObservableCollection<CategoryItemViewModel>();
        RebuildFlatList();

        eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Subscribe(DeleteCategory);
        eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(SetActiveCategory);
        eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Subscribe(OnCategoryNameUpdated);
        eventAggregator.GetEvent<CategoryRestoredEvent>().Subscribe(OnCategoryRestored);
        eventAggregator.GetEvent<CategoryToggleExpandEvent>().Subscribe(OnToggleExpand);
        eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Subscribe(OnAddSubcategory);
        eventAggregator.GetEvent<CategoryRenameClickedEvent>().Subscribe(OnRenameClicked);
        eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Subscribe(OnMoveToRoot);
        eventAggregator.GetEvent<CategoryMovedEvent>().Subscribe(OnCategoryMoved);
        eventAggregator.GetEvent<CategoryMakeSubcategoryClickedEvent>().Subscribe(OnMakeSubcategory);
    }

    public int RecycleBinCategoryId => Constants.RecycleBinCategoryId;
    public string? PendingAddNewCategoryText { get; set; }
    public ICommand AddCategoryCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenNoteListPageCommand { get; }
    public ICommand OpenRecycleBinPageCommand { get; }
    public ObservableCollection<CategoryItemViewModel> FlattenedItems { get; }
    public int ActiveCategoryId { get; private set; }

    public IEnumerable<CategoryItemViewModel> AllCategories => GetAllCategoriesFlat();

    public IEnumerable<CategoryItemViewModel> InactiveCategories =>
        GetAllCategoriesFlat().Where(c => c.Id != ActiveCategoryId);

    private IEnumerable<CategoryItemViewModel> GetAllCategoriesFlat()
    {
        var result = new List<CategoryItemViewModel>();
        CollectAll(_treeRoots, result);
        return result;
    }

    private static void CollectAll(IEnumerable<CategoryItemViewModel> items, List<CategoryItemViewModel> result)
    {
        foreach (var item in items)
        {
            result.Add(item);
            CollectAll(item.Children, result);
        }
    }

    private void RebuildFlatList()
    {
        FlattenedItems.Clear();
        FlattenVisible(_treeRoots, FlattenedItems);
        OnPropertyChanged(nameof(InactiveCategories));
        OnPropertyChanged(nameof(AllCategories));
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

    private CategoryItemViewModel? FindInTree(int categoryId)
    {
        return FindInTreeRecursive(_treeRoots, categoryId);
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

    private CategoryItemViewModel? FindParentOf(int categoryId)
    {
        return FindParentRecursive(_treeRoots, categoryId);
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

    private void EnsureAncestorsExpanded(int? parentCategoryId)
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

    private void RemoveFromTree(int categoryId)
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

    private void AddCategory()
    {
        PendingAddNewCategoryText = PendingAddNewCategoryText?.Trim();

        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            return;
        }

        var existingCategory = _categoriesRepository.GetCategoryByName(PendingAddNewCategoryText, null);

        if (existingCategory != null)
        {
            if (existingCategory.IsDeleted)
            {
                _mediator.Send(new RestoreCategoryCommand { Id = existingCategory.Id });
            }
            else
            {
                _mediator.Send(new ShowMessageWarningCommand { Message = "A root category with this name already exists!" });
            }
        }
        else
        {
            AddNewCategory(null);
        }

        PendingAddNewCategoryText = string.Empty;
    }

    private void AddNewCategory(int? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            throw new InvalidOperationException("Cannot add category with empty name");
        }

        int lastListOrder;
        if (parentCategoryId.HasValue)
        {
            var siblings = _categoriesRepository.GetChildCategories(parentCategoryId.Value);
            lastListOrder = siblings.LastOrDefault()?.ListOrder ?? Constants.DefaultListOrder;
        }
        else
        {
            var rootItems = _categoriesRepository.GetRootCategories();
            lastListOrder = rootItems.LastOrDefault()?.ListOrder ?? Constants.DefaultListOrder;
        }

        var addedCategory = _categoriesRepository.AddCategory(
            new Category
            {
                Name = PendingAddNewCategoryText,
                ParentCategoryId = parentCategoryId,
                ListOrder = lastListOrder + 1
            });

        var vm = addedCategory.MapToViewModel(_eventAggregator);
        vm.Depth = parentCategoryId.HasValue ? (FindInTree(parentCategoryId.Value)?.Depth ?? 0) + 1 : 0;

        if (parentCategoryId.HasValue)
        {
            var parentVm = FindInTree(parentCategoryId.Value);
            if (parentVm != null)
            {
                parentVm.Children.Add(vm);
                parentVm.HasChildren = true;
                parentVm.IsExpanded = true;
            }
        }
        else
        {
            _treeRoots.Add(vm);
        }

        RebuildFlatList();
    }

    private void OnAddSubcategory(int parentCategoryId)
    {
        var baseName = "New subcategory";
        var name = baseName;
        var counter = 1;

        while (_categoriesRepository.GetCategoryByName(name, parentCategoryId) is { IsDeleted: false })
        {
            name = $"{baseName} ({counter++})";
        }

        PendingAddNewCategoryText = name;
        AddNewCategory(parentCategoryId);
        PendingAddNewCategoryText = string.Empty;

        // Put the new subcategory into rename mode
        var newCategory = FindInTree(_categoriesRepository.GetCategoryByName(name, parentCategoryId)?.Id ?? -1);
        if (newCategory != null)
        {
            newCategory.RenameText = newCategory.Name;
            newCategory.IsRenaming = true;
        }
    }

    private void OnCategoryRestored(int categoryId)
    {
        var dbCategory = _categoriesRepository.GetCategoryById(categoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        var vm = dbCategory.MapToViewModel(_eventAggregator);

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

    private void DeleteCategory(int categoryId)
    {
        var category = FindInTree(categoryId);
        ArgumentNullException.ThrowIfNull(category);

        var activeCategories = _categoriesRepository.GetActiveCategories();
        if (activeCategories.Count <= 1)
        {
            _mediator.Send(new ShowMessageErrorCommand { Message = "Cannot delete last category." });
            return;
        }

        // Delete tasks in this category and all descendants
        var descendantIds = _categoriesRepository.GetDescendantCategoryIds(categoryId);
        _mediator.Send(new DeleteTaskItemsInCategoryCommand { CategoryId = category.Id });
        foreach (var descId in descendantIds)
        {
            _mediator.Send(new DeleteTaskItemsInCategoryCommand { CategoryId = descId });
        }

        RemoveFromTree(categoryId);
        _categoriesRepository.DeleteCategory(category.Map());

        _mediator.Send(new ShowMessageInfoCommand { Message = $"Deleted category: {category.Name}" });

        _eventAggregator.GetEvent<CategoryDeletedEvent>().Publish(categoryId);
        foreach (var descId in descendantIds)
        {
            _eventAggregator.GetEvent<CategoryDeletedEvent>().Publish(descId);
        }

        RebuildFlatList();

        if (category.Id == ActiveCategoryId || descendantIds.Contains(ActiveCategoryId))
        {
            var allFlat = GetAllCategoriesFlat().ToList();
            if (allFlat.Count > 0)
            {
                SetActiveCategory(allFlat.First().Id);
            }
        }
    }

    private void SetActiveCategory(int categoryId)
    {
        CategoryItemViewModel? category;
        if (categoryId == Constants.RecycleBinCategoryId)
        {
            var recycleBinCategory = _categoriesRepository.GetCategoryById(categoryId);
            ArgumentNullException.ThrowIfNull(recycleBinCategory);
            category = recycleBinCategory.MapToViewModel(_eventAggregator);
        }
        else
        {
            category = FindInTree(categoryId);
            ArgumentNullException.ThrowIfNull(category);

            EnsureAncestorsExpanded(category.ParentCategoryId);
            RebuildFlatList();
        }

        if (ActiveCategoryId != category.Id)
        {
            ActiveCategoryId = category.Id;
            AppSettings.Instance.SessionSettings.ActiveCategoryId = ActiveCategoryId;
        }

        _mediator.Publish(new ActiveCategoryChangedEvent
        {
            CategoryId = category.Id,
            CategoryName = category.Name
        });
    }

    private void OnToggleExpand(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null) return;

        category.IsExpanded = !category.IsExpanded;
        RebuildFlatList();
    }

    private void OnRenameClicked(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null) return;

        category.RenameText = category.Name;
        category.IsRenaming = true;
    }

    public void FinishRename(int categoryId, string? newName)
    {
        var category = FindInTree(categoryId);
        if (category == null) return;

        category.IsRenaming = false;

        if (string.IsNullOrWhiteSpace(newName) || newName == category.Name)
        {
            return;
        }

        _mediator.Send(new RenameCategoryCommand { CategoryId = categoryId, Name = newName.Trim() });
    }

    public void CancelRename(int categoryId)
    {
        var category = FindInTree(categoryId);
        if (category == null) return;

        category.IsRenaming = false;
    }

    private void OnMoveToRoot(int categoryId)
    {
        _mediator.Send(new MoveCategoryCommand { CategoryId = categoryId, NewParentCategoryId = null });
    }

    private void OnMakeSubcategory(CategoryMakeSubcategoryPayload payload)
    {
        _mediator.Send(new MoveCategoryCommand
        {
            CategoryId = payload.CategoryId,
            NewParentCategoryId = payload.NewParentCategoryId
        });
    }

    private void OnCategoryMoved(CategoryMovedPayload payload)
    {
        ReloadTree();
    }

    private void ReloadTree()
    {
        var activeCategories = _categoriesRepository.GetActiveCategories();
        _treeRoots = activeCategories.BuildTree(_eventAggregator);

        // Restore expand states from the old flattened list won't be possible after full reload,
        // so we expand ancestors of the active category
        EnsureAncestorsExpanded(FindInTree(ActiveCategoryId)?.ParentCategoryId);

        RebuildFlatList();
    }

    private void OpenSettingsPage()
    {
        _overlayPageNavigationService.NavigateTo<ISettingsPage>();
    }

    private void OpenNoteListPage()
    {
        // TODO:
        //_sideMenuPageNavigationService.NavigateTo<INoteListPage>();
    }

    private void OpenRecycleBinPage()
    {
        SetActiveCategory(Constants.RecycleBinCategoryId);
    }

    private void OnCategoryNameUpdated(CategoryNameUpdatedPayload payload)
    {
        var category = FindInTree(payload.CategoryId);
        if (category != null)
        {
            category.Name = payload.CategoryName;
            RebuildFlatList();
        }
    }

    protected override void OnDispose()
    {
        _eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Unsubscribe(DeleteCategory);
        _eventAggregator.GetEvent<CategoryClickedEvent>().Unsubscribe(SetActiveCategory);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Unsubscribe(OnCategoryNameUpdated);
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Unsubscribe(OnCategoryRestored);
        _eventAggregator.GetEvent<CategoryToggleExpandEvent>().Unsubscribe(OnToggleExpand);
        _eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Unsubscribe(OnAddSubcategory);
        _eventAggregator.GetEvent<CategoryRenameClickedEvent>().Unsubscribe(OnRenameClicked);
        _eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Unsubscribe(OnMoveToRoot);
        _eventAggregator.GetEvent<CategoryMovedEvent>().Unsubscribe(OnCategoryMoved);
        _eventAggregator.GetEvent<CategoryMakeSubcategoryClickedEvent>().Unsubscribe(OnMakeSubcategory);
    }
}
