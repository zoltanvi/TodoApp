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

    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
    
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
    public ICollectionView ItemsView { get; set; }

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
        RestoreFullCategoryCommand = new RelayCommand(() => _mediator.Send(new RestoreTaskItemsInCategoryCommand { CategoryId = CategoryId }));

        Items = items;
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterTaskItem;

        IsOpen = isOpen;
    }

    private bool FilterTaskItem(object obj)
    {
        return IsOpen;
    }
}
