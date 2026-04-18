using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Modules.Categories.Views.DragDrop;
using Modules.Categories.Views.Events;
using Modules.Categories.Views.Mappings;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Events;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts;
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
    private readonly IPopupMessageService _popupMessageService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IAppSettings _appSettings;
    private readonly CategoryPageTreeState _treeState;

    public CategoryPageViewModel(
        ICategoriesRepository categoriesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IOverlayPageNavigationService overlayPageNavigationService,
        IMediator mediator,
        IPopupMessageService popupMessageService,
        IEventAggregator eventAggregator,
        IAppSettings appSettings,
        TaskToCategoryDropHandler categoryDropHandler)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigationService);
        ArgumentNullException.ThrowIfNull(overlayPageNavigationService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(popupMessageService);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(appSettings);
        ArgumentNullException.ThrowIfNull(categoryDropHandler);

        _categoriesRepository = categoriesRepository;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;
        _overlayPageNavigationService = overlayPageNavigationService;
        _mediator = mediator;
        _popupMessageService = popupMessageService;
        _eventAggregator = eventAggregator;
        _appSettings = appSettings;
        CategoryDropHandler = categoryDropHandler;

        _treeState = new CategoryPageTreeState(
            categoriesRepository,
            eventAggregator,
            appSettings,
            () =>
            {
                OnPropertyChanged(nameof(InactiveCategories));
                OnPropertyChanged(nameof(AllCategories));
            });

        AddCategoryCommand = new RelayCommand(AddCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        ActiveCategoryId = _appSettings.SessionSettings.ActiveCategoryId;

        _treeState.Initialize();

        eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Subscribe(DeleteCategory);
        eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(SetActiveCategory);
        eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Subscribe(OnCategoryNameUpdated);
        eventAggregator.GetEvent<CategoryRestoredEvent>().Subscribe(OnCategoryRestored);
        eventAggregator.GetEvent<CategoryToggleExpandEvent>().Subscribe(OnToggleExpand);
        eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Subscribe(OnAddSubcategory);
        eventAggregator.GetEvent<HotkeyPressedCtrlShiftNEvent>().Subscribe(OnHotkeyAddSubcategory);
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
    public ObservableCollection<CategoryItemViewModel> FlattenedItems => _treeState.FlattenedItems;
    public TaskToCategoryDropHandler CategoryDropHandler { get; }
    public int ActiveCategoryId { get; private set; }
    public int FocusedCategoryId { get; set; } = -1;

    public IEnumerable<CategoryItemViewModel> AllCategories => _treeState.GetAllCategoriesFlat();

    public IEnumerable<CategoryItemViewModel> InactiveCategories =>
        _treeState.GetAllCategoriesFlat().Where(c => c.Id != ActiveCategoryId);

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
                _popupMessageService.ShowWarning("A root category with this name already exists!");
            }
        }
        else
        {
            _ = AddNewCategory(null);
        }

        PendingAddNewCategoryText = string.Empty;
    }

    private Category AddNewCategory(int? parentCategoryId)
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
        _treeState.AddNewCategoryViewModel(vm, parentCategoryId);
        return addedCategory;
    }

    private void OnHotkeyAddSubcategory()
    {
        var parentId = ResolveSubcategoryParentCategoryId();
        if (parentId < 0) return;

        OnAddSubcategory(parentId);
    }

    /// <summary>
    /// Focused category if set and not recycle bin; otherwise the active category if not recycle bin.
    /// </summary>
    private int ResolveSubcategoryParentCategoryId()
    {
        if (FocusedCategoryId >= 0 && FocusedCategoryId != Constants.RecycleBinCategoryId)
            return FocusedCategoryId;

        if (ActiveCategoryId != Constants.RecycleBinCategoryId)
            return ActiveCategoryId;

        return -1;
    }

    private void OnAddSubcategory(int parentCategoryId)
    {
        var baseName = "New subcategory";
        var name = baseName;
        var counter = 1;

        while (_categoriesRepository.ActiveCategoryExistsWithName(name, parentCategoryId))
        {
            name = $"{baseName} ({counter++})";
        }

        PendingAddNewCategoryText = name;
        var added = AddNewCategory(parentCategoryId);
        PendingAddNewCategoryText = string.Empty;

        // Put the new subcategory into rename mode
        var newCategory = _treeState.FindInTree(added.Id);
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
        _treeState.ApplyRestoredCategory(dbCategory, vm);
    }

    private void DeleteCategory(int categoryId)
    {
        var category = _treeState.FindInTree(categoryId);
        ArgumentNullException.ThrowIfNull(category);

        var activeCategories = _categoriesRepository.GetActiveCategories();
        if (activeCategories.Count <= 1)
        {
            _popupMessageService.ShowError("Cannot delete last category.");
            return;
        }

        // Delete tasks in this category and all descendants
        var descendantIds = _categoriesRepository.GetDescendantCategoryIds(categoryId);
        _mediator.Send(new DeleteTaskItemsInCategoryCommand { CategoryId = category.Id });
        foreach (var descId in descendantIds)
        {
            _mediator.Send(new DeleteTaskItemsInCategoryCommand { CategoryId = descId });
        }

        _treeState.RemoveFromTree(categoryId);
        _categoriesRepository.DeleteCategory(category.Map());

        _popupMessageService.ShowInfo($"Deleted category: {category.Name}");

        _eventAggregator.GetEvent<CategoryDeletedEvent>().Publish(categoryId);
        foreach (var descId in descendantIds)
        {
            _eventAggregator.GetEvent<CategoryDeletedEvent>().Publish(descId);
        }

        _treeState.RebuildFlatList();

        if (category.Id == ActiveCategoryId || descendantIds.Contains(ActiveCategoryId))
        {
            var allFlat = _treeState.GetAllCategoriesFlat().ToList();
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
            category = _treeState.FindInTree(categoryId);
            ArgumentNullException.ThrowIfNull(category);

            _treeState.EnsureAncestorsExpanded(category.ParentCategoryId);
            _treeState.SaveExpandedStates();
            _treeState.RebuildFlatList();
        }

        if (ActiveCategoryId != category.Id)
        {
            ActiveCategoryId = category.Id;
            _appSettings.SessionSettings.ActiveCategoryId = ActiveCategoryId;
        }

        FocusedCategoryId = category.Id;

        _eventAggregator.GetEvent<ActiveCategoryChangedEvent>().Publish(new ActiveCategoryChangedPayload
        {
            CategoryId = category.Id,
            CategoryName = category.Name
        });
    }

    public void ExpandCategory(int categoryId) => _treeState.ExpandCategory(categoryId);

    public void CollapseCategory(int categoryId) => _treeState.CollapseCategory(categoryId);

    public void ActivateCategory(int categoryId)
    {
        SetActiveCategory(categoryId);
    }

    /// <summary>
    /// When a normal (non-recycle-bin) category is active, moves focus to the task page new-task editor.
    /// </summary>
    public void MoveFocusToTaskPageNewTaskInput()
    {
        if (ActiveCategoryId == Constants.RecycleBinCategoryId) return;

        _eventAggregator.GetEvent<FocusTaskPageNewTaskEditorEvent>().Publish();
    }

    public int GetFocusedIndex()
    {
        if (FocusedCategoryId < 0) return -1;

        for (int i = 0; i < FlattenedItems.Count; i++)
        {
            if (FlattenedItems[i].Id == FocusedCategoryId) return i;
        }

        return -1;
    }

    private void OnToggleExpand(int categoryId) => _treeState.ToggleExpand(categoryId);

    private void OnRenameClicked(int categoryId)
    {
        var category = _treeState.FindInTree(categoryId);
        if (category == null) return;

        category.RenameText = category.Name;
        category.IsRenaming = true;
    }

    public void FinishRename(int categoryId, string? newName)
    {
        var category = _treeState.FindInTree(categoryId);
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
        var category = _treeState.FindInTree(categoryId);
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
        _treeState.ReloadTree(ActiveCategoryId);
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
        var category = _treeState.FindInTree(payload.CategoryId);
        if (category != null)
        {
            category.Name = payload.CategoryName;
            _treeState.RebuildFlatList();
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
        _eventAggregator.GetEvent<HotkeyPressedCtrlShiftNEvent>().Unsubscribe(OnHotkeyAddSubcategory);
        _eventAggregator.GetEvent<CategoryRenameClickedEvent>().Unsubscribe(OnRenameClicked);
        _eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Unsubscribe(OnMoveToRoot);
        _eventAggregator.GetEvent<CategoryMovedEvent>().Unsubscribe(OnCategoryMoved);
        _eventAggregator.GetEvent<CategoryMakeSubcategoryClickedEvent>().Unsubscribe(OnMakeSubcategory);
    }
}
