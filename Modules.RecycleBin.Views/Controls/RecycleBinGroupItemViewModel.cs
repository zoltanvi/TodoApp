using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.RecycleBin.Views.Controls;

public class RecycleBinGroupItemViewModel : BaseViewModel
{
    public RecycleBinGroupItemViewModel()
    {
        ToggleGroupIsOpen = new RelayCommand(() => IsOpen ^= true);
    }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
    public ICommand ToggleGroupIsOpen { get; }
    public bool IsOpen { get; set; } = true;
}
