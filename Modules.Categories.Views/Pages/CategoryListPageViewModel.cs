using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Modules.Categories.Views.Mappings;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts.Cqrs.Commands;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Modules.Categories.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class CategoryListPageViewModel : BaseViewModel
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;
    private readonly IMediator _mediator;

    public CategoryListPageViewModel(
        ICategoriesRepository categoriesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigationService);
        ArgumentNullException.ThrowIfNull(mediator);

        _categoriesRepository = categoriesRepository;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;
        _mediator = mediator;

        AddCategoryCommand = new RelayCommand(AddCategory);
        DeleteCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(DeleteCategory);
        ChangeCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(SetActiveCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        var activeCategories = categoriesRepository.GetActiveCategories();
        ActiveCategoryId = AppSettings.Instance.SessionSettings.ActiveCategoryId;

        Items = new ObservableCollection<CategoryViewModel>(activeCategories.MapToViewModelList());
        Items.CollectionChanged += ItemsOnCollectionChanged;
        CategoryNameUpdatedEvent.CategoryNameUpdated += OnCategoryNameUpdated;
        RestoreCategoryRequestedEvent.RestoreCategoryRequested += OnRestoreCategoryRequested;
    }

    public int RecycleBinCategoryId => Constants.RecycleBinCategoryId;
    public string? PendingAddNewCategoryText { get; set; }
    public ICommand AddCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenNoteListPageCommand { get; }
    public ICommand OpenRecycleBinPageCommand { get; }
    public ObservableCollection<CategoryViewModel> Items { get; set; }
    public IEnumerable<CategoryViewModel> InactiveCategories => Items.Where(c => c.Id != ActiveCategoryId);
    public int ActiveCategoryId { get; private set; }

    private void AddCategory()
    {
        // Remove trailing and leading whitespaces
        PendingAddNewCategoryText = PendingAddNewCategoryText?.Trim();

        // If the text is empty or only whitespace, refuse
        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            return;
        }

        // Untrash category if exists
        var existingCategory = _categoriesRepository.GetCategoryByName(PendingAddNewCategoryText);

        if (existingCategory != null && existingCategory.IsDeleted)
        {
            RestoreCategory(existingCategory);
        }
        else
        {
            AddNewCategory();
        }

        // Reset the input TextBox text
        PendingAddNewCategoryText = string.Empty;
    }

    private void AddNewCategory()
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            throw new InvalidOperationException("Cannot add category with empty name");
        }

        var activeItems = _categoriesRepository.GetActiveCategories();
        var lastListOrder = activeItems.LastOrDefault()?.ListOrder ?? Constants.DefaultListOrder;

        var addedCategory = _categoriesRepository.AddCategory(
            new Category
            {
                Name = PendingAddNewCategoryText,
                ListOrder = lastListOrder + 1
            });

        Items.Add(addedCategory.MapToViewModel());
    }

    private void RestoreCategory(Category category)
    {
        _categoriesRepository.RestoreCategory(category, Items.Count);

        Items.Add(category.MapToViewModel());
    }

    private void OnRestoreCategoryRequested(RestoreCategoryRequestedEvent e)
    {
        var dbCategory = _categoriesRepository.GetCategoryById(e.CategoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        RestoreCategory(dbCategory);
    }

    private void DeleteCategory(CategoryViewModel category)
    {
        // At least one category is required
        var activeCategories = _categoriesRepository.GetActiveCategories();
        if (activeCategories.Count <= 1)
        {
            _mediator.Send(new ShowMessageErrorCommand { Message = "Cannot delete last category." });

            return;
        }

        _mediator.Send(new DeleteTaskItemsInCategoryCommand { CategoryId = category.Id });

        Items.Remove(category);
        _categoriesRepository.DeleteCategory(category.Map());

        _mediator.Send(new ShowMessageInfoCommand { Message = $"Deleted category: {category.Name}" });
        
        CategoryDeletedEvent.Invoke(new CategoryDeletedEvent { CategoryId = category.Id });

        // Only if the current category was the deleted one, select a new category
        if (category.Id == ActiveCategoryId)
        {
            SetActiveCategory(Items.FirstOrDefault());
        }
    }

    private void SetActiveCategory(CategoryViewModel? category)
    {
        if (string.IsNullOrWhiteSpace(category?.Name)) return;

        if (ActiveCategoryId != category.Id)
        {
            ActiveCategoryId = category.Id;

            AppSettings.Instance.SessionSettings.ActiveCategoryId = ActiveCategoryId;

            _mediator.Publish(new ActiveCategoryChangedEvent
            {
                CategoryId = category.Id,
                CategoryName = category.Name
            });

            //IoC.NoteListService.ActiveNote = null;
        }
    }

    private void OpenSettingsPage()
    {
        _mainPageNavigationService.NavigateTo<ISettingsPage>();

        if (AppSettings.Instance.ApplicationSettings.CloseSideMenuOnPageChange)
        {
            AppSettings.Instance.SessionSettings.SideMenuOpen = false;
        }
    }

    private void OpenNoteListPage()
    {
        _sideMenuPageNavigationService.NavigateTo<INoteListPage>();
    }

    private void OpenRecycleBinPage()
    {
        var recycleBinCategory = _categoriesRepository.GetCategoryById(Constants.RecycleBinCategoryId);

        SetActiveCategory(recycleBinCategory?.MapToViewModel());
    }

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
        }

        _categoriesRepository.UpdateCategoryListOrders(Items.MapList());

        // Trigger update to refresh move to category items context menu
        OnPropertyChanged(nameof(InactiveCategories));
    }

    private void OnCategoryNameUpdated(CategoryNameUpdatedEvent e)
    {
        var oldCategory = Items.FirstOrDefault(x => x.Id == e.Id);

        if (oldCategory != null)
        {
            oldCategory.Name = e.CategoryName;
            var index = Items.IndexOf(oldCategory);
            Items.RemoveAt(index);
            Items.Insert(index, oldCategory);
        }
    }

    protected override void OnDispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;
        CategoryNameUpdatedEvent.CategoryNameUpdated -= OnCategoryNameUpdated;
        RestoreCategoryRequestedEvent.RestoreCategoryRequested -= OnRestoreCategoryRequested;
    }
}