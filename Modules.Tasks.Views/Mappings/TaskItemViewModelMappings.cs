using MediatR;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Services;

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
            ContentPreview = vm.ContentPreview,
            ListOrder = vm.ListOrder,
            Pinned = vm.Pinned,
            IsDone = vm.IsDone,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            MarkerColor = vm.MarkerColor,
            BorderColor = vm.BorderColor,
            BackgroundColor = vm.BackgroundColor,
            IsDeleted = vm.IsDeleted,
            DeletedDate = vm.DeletedDate,
            Versions = vm.Versions.MapList()
        };
    }

    public static List<TaskItem> MapList(this IEnumerable<TaskItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TaskItemViewModel MapToViewModel(
        this TaskItem taskItem, 
        IMediator mediator,
        OneEditorOpenService oneEditorOpenService)
    {
        return new TaskItemViewModel(mediator, oneEditorOpenService)
        {
            Id = taskItem.Id,
            CategoryId = taskItem.CategoryId,
            Content = taskItem.Content,
            ContentPreview = taskItem.ContentPreview,
            ListOrder = taskItem.ListOrder,
            Pinned = taskItem.Pinned,
            IsDone = taskItem.IsDone,
            CreationDate = taskItem.CreationDate,
            ModificationDate = taskItem.ModificationDate,
            MarkerColor = taskItem.MarkerColor,
            BorderColor = taskItem.BorderColor,
            BackgroundColor = taskItem.BackgroundColor,
            IsDeleted = taskItem.IsDeleted,
            DeletedDate = taskItem.DeletedDate,
            Versions = taskItem.Versions.MapToViewModelList()
        };
    }

    public static List<TaskItemViewModel> MapToViewModelList(
        this IEnumerable<TaskItem> taskList, 
        IMediator mediator,
        OneEditorOpenService oneEditorOpenService) =>
        taskList.Select(x => x.MapToViewModel(mediator, oneEditorOpenService)).ToList();
}
