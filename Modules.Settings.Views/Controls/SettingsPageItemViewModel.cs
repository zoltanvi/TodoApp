using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Settings.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class SettingsPageItemViewModel : BaseViewModel
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required Action NavigateAction { get; set; }
}
