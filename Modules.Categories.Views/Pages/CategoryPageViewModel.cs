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
using System.Collections.Specialized;
using System.Windows.Input;

namespace Modules.Categories.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class CategoryPageViewModel : BaseViewModel
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public CategoryPageViewModel(
        ICategoriesRepository categoriesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigationService);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        
        _categoriesRepository = categoriesRepository;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;
        _mediator = mediator;
        _eventAggregator = eventAggregator;

        AddCategoryCommand = new RelayCommand(AddCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        var activeCategories = categoriesRepository.GetActiveCategories();
        ActiveCategoryId = AppSettings.Instance.SessionSettings.ActiveCategoryId;

        Items = new ObservableCollection<CategoryItemViewModel>(activeCategories.MapToViewModelList(_eventAggregator));
        Items.CollectionChanged += ItemsOnCollectionChanged;

        eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Subscribe(DeleteCategory);
        eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(SetActiveCategory);
        eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Subscribe(OnCategoryNameUpdated);
        eventAggregator.GetEvent<CategoryRestoredEvent>().Subscribe(OnCategoryRestored);
    }

    public int RecycleBinCategoryId => Constants.RecycleBinCategoryId;
    public string? PendingAddNewCategoryText { get; set; }
    public ICommand AddCategoryCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenNoteListPageCommand { get; }
    public ICommand OpenRecycleBinPageCommand { get; }
    public ObservableCollection<CategoryItemViewModel> Items { get; }
    public IEnumerable<CategoryItemViewModel> InactiveCategories => Items.Where(c => c.Id != ActiveCategoryId);
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

        if (existingCategory != null)
        {
            if (existingCategory.IsDeleted)
            {
                _mediator.Send(new RestoreCategoryCommand { Id = existingCategory.Id });
            }
            else
            {
                _mediator.Send(new ShowMessageWarningCommand { Message = "A category with this name already exists!" });
            }
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

        Items.Add(addedCategory.MapToViewModel(_eventAggregator));
    }

    private void OnCategoryRestored(int categoryId)
    {
        var dbCategory = _categoriesRepository.GetCategoryById(categoryId);
        ArgumentNullException.ThrowIfNull(dbCategory);

        Items.Add(dbCategory.MapToViewModel(_eventAggregator));
    }

    private void DeleteCategory(int categoryId)
    {
        var category = Items.FirstOrDefault(x => x.Id == categoryId);
        ArgumentNullException.ThrowIfNull(category);

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
        
        _eventAggregator.GetEvent<CategoryDeletedEvent>().Publish(categoryId);

        // Only if the current category was the deleted one, select a new category
        if (category.Id == ActiveCategoryId)
        {
            SetActiveCategory(Items.First().Id);
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
            category = Items.FirstOrDefault(x => x.Id == categoryId);
            ArgumentNullException.ThrowIfNull(category);
        }

        if (ActiveCategoryId != category.Id)
        {
            ActiveCategoryId = category.Id;

            AppSettings.Instance.SessionSettings.ActiveCategoryId = ActiveCategoryId;

            //IoC.NoteListService.ActiveNote = null;
        }

        _mediator.Publish(new ActiveCategoryChangedEvent
        {
            CategoryId = category.Id,
            CategoryName = category.Name
        });
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
        // TODO:
        //_sideMenuPageNavigationService.NavigateTo<INoteListPage>();
    }

    private void OpenRecycleBinPage()
    {
        SetActiveCategory(Constants.RecycleBinCategoryId);
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

    private void OnCategoryNameUpdated(CategoryNameUpdatedPayload payload)
    {
        var oldCategory = Items.FirstOrDefault(x => x.Id == payload.CategoryId);

        if (oldCategory != null)
        {
            oldCategory.Name = payload.CategoryName;
            var index = Items.IndexOf(oldCategory);
            Items.RemoveAt(index);
            Items.Insert(index, oldCategory);
        }
    }

    protected override void OnDispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;

        _eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Unsubscribe(DeleteCategory);
        _eventAggregator.GetEvent<CategoryClickedEvent>().Unsubscribe(SetActiveCategory);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Unsubscribe(OnCategoryNameUpdated);
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Unsubscribe(OnCategoryRestored);
    }
}