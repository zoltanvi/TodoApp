using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TagSelectionItemViewModel : BaseViewModel
{
    private readonly TagSelectorPageViewModel _tagSelectorPageViewModel;

    public TagSelectionItemViewModel(TagSelectorPageViewModel tagSelectorPageViewModel)
    {
        // TODO: Change it to mediator pattern by using prism
        _tagSelectorPageViewModel = tagSelectorPageViewModel;
        SelectTagCommand = new RelayCommand(() => _tagSelectorPageViewModel.SelectTag(Id));
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public TagPresetColor Color { get; set; }
    public bool IsSelected { get; set; }

    public ICommand SelectTagCommand { get; }
}
