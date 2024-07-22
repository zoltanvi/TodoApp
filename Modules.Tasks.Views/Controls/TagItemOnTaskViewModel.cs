using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TagItemOnTaskViewModel : BaseViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public TagPresetColor Color { get; set; }
}
