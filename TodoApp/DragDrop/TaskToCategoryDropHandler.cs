using GongSolutions.Wpf.DragDrop;
using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Views.Controls;
using Modules.Common.Views.DragDrop;
using Modules.Common.Views.Services;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Views.Controls.TaskItemView;
using DragDropEffects = System.Windows.DragDropEffects;

namespace TodoApp.DragDrop;

/// <summary>
/// Handles dropping a [task on a category], drag n drop a [category next to another category],
/// or dropping a [category onto another category] to make it a subcategory.
/// </summary>
public class TaskToCategoryDropHandler : DefaultDropHandler
{
    public static TaskToCategoryDropHandler Instance { get; } = new();

    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: TaskItemViewModel, TargetItem: CategoryItemViewModel })
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        }
        else if (dropInfo is { Data: CategoryItemViewModel source, TargetItem: CategoryItemViewModel target }
                 && source.Id != target.Id)
        {
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

            mediator.Send(new MoveTaskToNewCategoryCommand { TaskId = task.Id, CategoryId = category.Id });
        }
        else if (dropInfo is { Data: CategoryItemViewModel sourceCategory, TargetItem: CategoryItemViewModel targetCategory }
                 && sourceCategory.Id != targetCategory.Id)
        {
            var mediator = ServiceLocator.GetService<IMediator>();
            ArgumentNullException.ThrowIfNull(mediator);

            mediator.Send(new MoveCategoryCommand
            {
                CategoryId = sourceCategory.Id,
                NewParentCategoryId = targetCategory.Id
            });
        }
        else
        {
            DragDropHelper.SimpleDrop(dropInfo);
        }
    }
}

