using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Settings.Views.Tag;

[AddINotifyPropertyChangedInterface]
public class TagItemViewModel : BaseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TagPresetColor Color { get; set; }
    public ICommand DeleteTagCommand { get; set; }

    public TagItemViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        DeleteTagCommand = new RelayCommand(() => mediator.Send(new DeleteTagItemCommand { TagId = Id }));
    }
}
