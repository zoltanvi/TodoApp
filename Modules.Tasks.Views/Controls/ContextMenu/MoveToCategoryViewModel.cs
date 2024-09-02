using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Tasks.Views.Controls.ContextMenu;

[AddINotifyPropertyChangedInterface]
public class MoveToCategoryViewModel : BaseViewModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
