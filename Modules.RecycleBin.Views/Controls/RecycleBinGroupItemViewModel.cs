using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Modules.RecycleBin.Views.Controls;

public class RecycleBinGroupItemViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private bool _isOpen;
    private HashSet<string> _searchTerms = new();

    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public bool IsDeletedCategory { get; init; }
    
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
    public ICollectionView ItemsView { get; set; }

    public ObservableCollection<RecycleBinGroupItemViewModel> Children { get; set; } = new();

    public ICommand ToggleGroupIsOpen { get; }
    public ICommand RestoreFullCategoryCommand { get; }

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            ItemsView.Refresh();
        }
    }

    public RecycleBinGroupItemViewModel(
        bool isOpen,
        ObservableCollection<RecycleBinTaskItemViewModel> items,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;
        ToggleGroupIsOpen = new RelayCommand(() => IsOpen ^= true);
        RestoreFullCategoryCommand = new RelayCommand(RestoreTree);

        Items = items;
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterTaskItem;

        IsOpen = isOpen;
    }

    private void RestoreTree()
    {
        if (Children.Count > 0)
        {
            _mediator.Send(new RestoreCategoryTreeCommand { RootCategoryId = CategoryId });
        }
        else
        {
            _mediator.Send(new RestoreTaskItemsInCategoryCommand { CategoryId = CategoryId });
        }
    }

    private bool FilterTaskItem(object obj)
    {
        if (_searchTerms.Count != 0 && obj is RecycleBinTaskItemViewModel taskItem)
        {
            var plainTextContent = taskItem.Content.GetContentInPlainText();
            return _searchTerms.All(x => plainTextContent.Contains(x, StringComparison.OrdinalIgnoreCase));
        }

        return IsOpen;
    }

    public bool SetSearchTerms(HashSet<string> searchTerms)
    {
        _searchTerms = searchTerms;
        ItemsView.Refresh();

        var hasOwnItems = ItemsView.Cast<object>().Any();
        var hasChildItems = Children.Any(c => c.SetSearchTerms(searchTerms));

        return hasOwnItems || hasChildItems;
    }

    public RecycleBinGroupItemViewModel? FindGroupInTree(int categoryId)
    {
        if (CategoryId == categoryId) return this;

        foreach (var child in Children)
        {
            var found = child.FindGroupInTree(categoryId);
            if (found != null) return found;
        }

        return null;
    }

    public RecycleBinTaskItemViewModel? FindTaskInTree(int taskId, out RecycleBinGroupItemViewModel? ownerGroup)
    {
        var task = Items.FirstOrDefault(x => x.Id == taskId);
        if (task != null)
        {
            ownerGroup = this;
            return task;
        }

        foreach (var child in Children)
        {
            task = child.FindTaskInTree(taskId, out ownerGroup);
            if (task != null) return task;
        }

        ownerGroup = null;
        return null;
    }

    public bool HasAnyContent()
    {
        return IsDeletedCategory || Items.Count > 0 || Children.Any(c => c.HasAnyContent());
    }

    public List<int> GetAllCategoryIds()
    {
        var result = new List<int> { CategoryId };
        foreach (var child in Children)
        {
            result.AddRange(child.GetAllCategoryIds());
        }
        return result;
    }
}
