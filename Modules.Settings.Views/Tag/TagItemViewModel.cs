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
    private readonly IMediator _mediator;
    private string _editableName;
    public int Id { get; set; }
    public string Name { get; set; }

    public string EditableName
    {
        get => _editableName;
        set
        {
            _editableName = value;

            NotSaved = Name != _editableName;
        }
    }

    public bool NotSaved { get; set; }

    public TagPresetColor Color { get; set; }
    public ICommand DeleteTagCommand { get; set; }
    public ICommand SaveModificationsCommand { get; set; }

    public TagItemViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;

        DeleteTagCommand = new RelayCommand(() => mediator.Send(new DeleteTagItemCommand { TagId = Id }));
        SaveModificationsCommand = new RelayCommand(SaveModifications);
    }

    private void SaveModifications()
    {
        if (NotSaved)
        {
            _mediator.Send(new UpdateTagItemCommand
            {
                TagId = Id,
                NewName = EditableName,
                Color = Color.ToString()
            });
        }
    }
}
