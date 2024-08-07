using MediatR;
using Modules.Common.DataModels;
using Modules.Common.Extensions;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Services;
using Prism.Events;
using System.Collections.ObjectModel;

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
        OneEditorOpenService oneEditorOpenService,
        IEventAggregator eventAggregator)
    {
        return new TaskItemViewModel(mediator, oneEditorOpenService, eventAggregator)
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
            Versions = taskItem.Versions.MapToViewModelList(mediator),
            Tags = taskItem.Tags.MapTagItems()
        };
    }

    public static List<TaskItemViewModel> MapToViewModelList(
        this IEnumerable<TaskItem> taskList, 
        IMediator mediator,
        OneEditorOpenService oneEditorOpenService,
        IEventAggregator eventAggregator) =>
        taskList.Select(x => x.MapToViewModel(mediator, oneEditorOpenService, eventAggregator)).ToList();

    public static ObservableCollection<TagItemOnTaskViewModel> MapTagItems(this IEnumerable<TagItem> tags)
    {
        return tags.Select(x => new TagItemOnTaskViewModel
        {
            Id = x.Id, Color = (TagPresetColor)Enum.Parse(typeof(TagPresetColor), x.Color), Name = x.Name
        }).ToObservableCollection();
    }
}
