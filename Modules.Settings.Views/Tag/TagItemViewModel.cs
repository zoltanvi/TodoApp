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
    private readonly TagPresetColor _originalColor;
    private readonly string _originalName;
    private TagPresetColor _color;
    private string _name;

    public TagItemViewModel(
        IMediator mediator,
        int id,
        string name,
        TagPresetColor color)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;
        
        Id = id;
        
        _originalName = name;
        Name = name;
        
        _originalColor = color;
        Color = color;

        DeleteTagCommand = new RelayCommand(() => mediator.Send(new DeleteTagItemCommand { TagId = Id }));
        SaveModificationsCommand = new RelayCommand(SaveModifications);
    }

    public int Id { get; set; }
    public bool NotSaved { get; set; }
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            CheckSaveState();
        }
    }

    public TagPresetColor Color
    {
        get => _color;
        set
        {
            _color = value;
            CheckSaveState();
        }
    }

    public ICommand DeleteTagCommand { get; set; }
    public ICommand SaveModificationsCommand { get; set; }

    private void CheckSaveState()
    {
        NotSaved = _originalColor != _color || _originalName != _name;
    }

    private void SaveModifications()
    {
        if (NotSaved && !string.IsNullOrWhiteSpace(Name))
        {
            _mediator.Send(new UpdateTagItemCommand
            {
                TagId = Id,
                NewName = Name,
                Color = Color.ToString()
            });
        }
        else
        {
            Name = _originalName;
        }
    }
}
