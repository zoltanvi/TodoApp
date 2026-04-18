using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Views.Controls;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Common.Views.DragDrop;
using Modules.Tasks.Views.Controls.TaskItemView;
using System.Collections;
using DragDropEffects = System.Windows.DragDropEffects;

namespace Modules.Categories.Views.DragDrop;

/// <summary>
/// Handles dropping a [task on a category], drag n drop a [category next to another category],
/// or dropping a [category onto another category] to make it a subcategory.
/// </summary>
public class TaskToCategoryDropHandler : DefaultDropHandler
{
    private readonly IMediator _mediator;

    public TaskToCategoryDropHandler(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;
    }

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
            _mediator.Send(new MoveTaskToNewCategoryCommand { TaskId = task.Id, CategoryId = category.Id });
        }
        else if (dropInfo is { Data: CategoryItemViewModel sourceCategory, TargetItem: CategoryItemViewModel targetCategory }
                 && sourceCategory.Id != targetCategory.Id)
        {
            _mediator.Send(new MoveCategoryCommand
            {
                CategoryId = sourceCategory.Id,
                NewParentCategoryId = targetCategory.Id
            });
        }
        else if (dropInfo is { Data: CategoryItemViewModel draggedCategory })
        {
            if (ShouldMoveToRoot(dropInfo, draggedCategory))
            {
                _mediator.Send(new MoveCategoryCommand
                {
                    CategoryId = draggedCategory.Id,
                    NewParentCategoryId = null
                });
            }
            else
            {
                DragDropHelper.SimpleDrop(dropInfo);
            }
        }
        else
        {
            DragDropHelper.SimpleDrop(dropInfo);
        }
    }

    private static bool ShouldMoveToRoot(IDropInfo dropInfo, CategoryItemViewModel draggedCategory)
    {
        if (!draggedCategory.IsSubcategory)
        {
            return false;
        }

        IList? targetList = dropInfo.TargetCollection?.TryGetList();
        if (targetList == null)
        {
            return false;
        }

        int insertIndex = dropInfo.UnfilteredInsertIndex;

        // Check the item before the insertion point
        if (insertIndex > 0 && insertIndex <= targetList.Count)
        {
            var itemBefore = targetList[insertIndex - 1] as CategoryItemViewModel;
            if (itemBefore != null && itemBefore.Depth == 0 && !itemBefore.IsExpanded)
            {
                return true;
            }
        }

        // Check the item at the insertion point (item after)
        if (insertIndex < targetList.Count)
        {
            var itemAfter = targetList[insertIndex] as CategoryItemViewModel;
            if (itemAfter != null && itemAfter.Depth == 0)
            {
                return true;
            }
        }

        // Dropping at the very end of the list - check if inserting after all root items
        if (insertIndex == targetList.Count && targetList.Count > 0)
        {
            var lastItem = targetList[targetList.Count - 1] as CategoryItemViewModel;
            if (lastItem != null && lastItem.Depth == 0)
            {
                return true;
            }
        }

        return false;
    }
}
