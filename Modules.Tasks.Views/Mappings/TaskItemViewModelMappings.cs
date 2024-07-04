using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;

namespace Modules.Tasks.Views.Mappings;

public static class TaskItemViewModelMappings
{
    public static TaskItem Map(this TaskItemViewModel vm)
    {
        return new TaskItem
        {
            Id = vm.Id,
            CategoryId = vm.CategoryId,
            Content = vm.Content,
            ContentPreview = "TODO",
            ListOrder = vm.ListOrder,
            Pinned = vm.Pinned,
            IsDone = vm.IsDone,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            MarkerColor = vm.MarkerColor,
            BorderColor = vm.BorderColor,
            BackgroundColor = vm.BackgroundColor,
            IsDeleted = vm.IsDeleted,
            DeletedDate = vm.DeletedDate
        };
    }

    public static List<TaskItem> MapList(this IEnumerable<TaskItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TaskItemViewModel MapToViewModel(this TaskItem taskItem)
    {
        return new TaskItemViewModel
        {
            Id = taskItem.Id,
            CategoryId = taskItem.CategoryId,
            Content = taskItem.Content,
            //ContentPreview = "TODO",
            ListOrder = taskItem.ListOrder,
            Pinned = taskItem.Pinned,
            IsDone = taskItem.IsDone,
            CreationDate = taskItem.CreationDate,
            ModificationDate = taskItem.ModificationDate,
            MarkerColor = taskItem.MarkerColor,
            BorderColor = taskItem.BorderColor,
            BackgroundColor = taskItem.BackgroundColor,
            IsDeleted = taskItem.IsDeleted,
            DeletedDate = taskItem.DeletedDate
        };
    }

    public static List<TaskItemViewModel> MapToViewModelList(this IEnumerable<TaskItem> taskList) =>
        taskList.Select(x => x.MapToViewModel()).ToList();
}
