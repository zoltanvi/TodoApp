using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemVersionViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public required string Content { get; set; }
    public required string ContentPreview { get; set; }
    public DateTime VersionDate { get; set; }

    public ICommand RestoreToThisCommand { get; }

    public TaskItemVersionViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        
        _mediator = mediator;

        RestoreToThisCommand = new RelayCommand(() => _mediator.Send(
            new RestoreTaskItemVersionCommand
            {
                TaskId = TaskId,
                VersionId = Id
            }));
    }
}
