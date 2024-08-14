using MediatR;
using Modules.RecycleBin.Views.Controls;
using Modules.Tasks.Contracts.Models;

namespace Modules.RecycleBin.Views.Mappings;

public static class RecycleBinTaskItemViewModelMappings
{
    public static RecycleBinTaskItemViewModel MapToRecycleBinTaskItem(this TaskItem taskItem, IMediator mediator)
    {
        return new RecycleBinTaskItemViewModel(
            mediator, 
            taskItem.Content, 
            taskItem.IsContentPlainText)
        {
            Id = taskItem.Id,
            CategoryId = taskItem.CategoryId,
            Pinned = taskItem.Pinned,
            IsDone = taskItem.IsDone,
            BackgroundColor = taskItem.BackgroundColor,
            BorderColor = taskItem.BorderColor,
            CreationDate = taskItem.CreationDate,
            DeletedDate = taskItem.DeletedDate,
            IsDeleted = taskItem.IsDeleted,
            ListOrder = taskItem.ListOrder,
            MarkerColor = taskItem.MarkerColor,
            ModificationDate = taskItem.ModificationDate,
            Versions = taskItem.Versions.MapToViewModelList()
        };
    }
}

