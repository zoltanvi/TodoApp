using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.TextEditor.Controls;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemVersionViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    public int Id { get; set; }
    public required int TaskId { get; set; }

    public DynamicTextBoxViewModel Content { get; set; }
    public DateTime VersionDate { get; set; }

    public ICommand RestoreToThisCommand { get; }

    public TaskItemVersionViewModel(
        IMediator mediator, 
        bool isContentPlainText, 
        string content)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        
        _mediator = mediator;

        Content = new DynamicTextBoxViewModel();
        Content.SetContent(isContentPlainText, content);

        RestoreToThisCommand = new RelayCommand(() => _mediator.Send(
            new RestoreTaskItemVersionCommand
            {
                TaskId = TaskId,
                VersionId = Id
            }));
    }
}
