using GongSolutions.Wpf.DragDrop;
using MediatR;
using Modules.Categories.Views.Controls;
using Modules.Common.Views.DragDrop;
using Modules.Common.Views.Services;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Views.Controls.TaskItemView;
using DragDropEffects = System.Windows.DragDropEffects;

namespace TodoApp.DragDrop;

/// <summary>
/// Handles dropping a [task on a category] or drag n drop a [category next to another category].
/// </summary>
public class TaskToCategoryDropHandler : DefaultDropHandler
{
    public static TaskToCategoryDropHandler Instance { get; } = new();

    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: TaskItemViewModel, TargetItem: CategoryItemViewModel })
        {
            // Set effects to show that the drop is allowed
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        }
        else
        {
            base.DragOver(dropInfo);
        }
    }

    public override void Drop(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: TaskItemViewModel task, TargetItem: CategoryItemViewModel category })
        {
            var mediator = ServiceLocator.GetService<IMediator>();
            ArgumentNullException.ThrowIfNull(mediator);

            mediator.Send(new MoveTaskToCategoryCommand { TaskId = task.Id, CategoryId = category.Id });
        }
        else
        {
            DragDropHelper.SimpleDrop(dropInfo);
        }
    }
}

