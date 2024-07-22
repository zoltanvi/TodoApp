using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.Tasks.Views.Events;
using Prism.Commands;
using Prism.Events;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TagSelectionItemViewModel : BaseViewModel
{
    public TagSelectionItemViewModel(IEventAggregator eventAggregator)
    {
        SelectTagCommand = new RelayCommand(() => eventAggregator.GetEvent<TagSelectionItemClickedEvent>().Publish(Id));
    }

    public int Id { get; init; }
    public required string Name { get; init; }
    public TagPresetColor Color { get; set; }
    public bool IsSelected { get; set; }
    public ICommand SelectTagCommand { get; }
}
