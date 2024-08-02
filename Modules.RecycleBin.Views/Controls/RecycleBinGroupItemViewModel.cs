using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Modules.RecycleBin.Views.Controls;

public class RecycleBinGroupItemViewModel : BaseViewModel
{
    private bool _isOpen;

    public RecycleBinGroupItemViewModel(
        bool isOpen,
        ObservableCollection<RecycleBinTaskItemViewModel> items)
    {
        ToggleGroupIsOpen = new RelayCommand(() => IsOpen ^= true);

        Items = items;
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterTaskItem;

        IsOpen = isOpen;
    }

    private bool FilterTaskItem(object obj)
    {
        return IsOpen;
    }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
    public ICollectionView ItemsView { get; set; }

    public ICommand ToggleGroupIsOpen { get; }

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            ItemsView.Refresh();
        }
    }
}
