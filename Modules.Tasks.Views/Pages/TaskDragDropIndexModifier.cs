using MediatR;
using Modules.Common.Views.DragDrop;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Views.Controls.TaskItemView;

namespace Modules.Tasks.Views.Pages;

/// <summary>
/// Computes Gong drag-drop insert index for task items using application rules.
/// </summary>
public sealed class TaskDragDropIndexModifier : IDropIndexModifier
{
    private readonly IMediator _mediator;

    public TaskDragDropIndexModifier(IMediator mediator)
    {
        _mediator = mediator;
    }

    public int GetModifiedDropIndex(int dropIndex, object droppedObject)
    {
        if (droppedObject is not TaskItemViewModel taskItem)
        {
            throw new ArgumentException($"{nameof(droppedObject)} is not a {nameof(TaskItemViewModel)}");
        }

        var query = new TaskDragDropInsertPositionQuery
        {
            TaskId = taskItem.Id,
            RequestedInsertPosition = dropIndex
        };

        return _mediator.Send(query).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
