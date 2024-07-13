using Modules.Common.ViewModel;
using System.Collections.ObjectModel;

namespace Modules.RecycleBin.Views.Controls;

public class RecycleBinGroupItemViewModel : BaseViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public ObservableCollection<RecycleBinTaskItemViewModel> Items { get; set; }
}
