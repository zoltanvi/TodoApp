using MediatR;
using Modules.Common.DataModels;
using Modules.Settings.Views.Tag;
using Modules.Tasks.Contracts.Models;

namespace Modules.Settings.Views.Mappings;

public static class TagItemViewModelMappings
{
    public static TagItem Map(this TagItemViewModel vm)
    {
        return new TagItem
        {
            Id = vm.Id,
            Name = vm.Name,
            Color = vm.Color.ToString(),
            //TaskItems = 
        };
    }

    public static List<TagItem> MapList(this IEnumerable<TagItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TagItemViewModel MapToViewModel(
        this TagItem tagItem,
        IMediator mediator)
    {
        return new TagItemViewModel(mediator)
        {
            Id = tagItem.Id,
            Name = tagItem.Name,
            EditableName = tagItem.Name,
            Color = (TagPresetColor)Enum.Parse(typeof(TagPresetColor), tagItem.Color),
        };
    }

    public static List<TagItemViewModel> MapToViewModelList(
        this IEnumerable<TagItem> tagList,
        IMediator mediator)
    {
        return tagList.Select(x => x.MapToViewModel(mediator)).ToList();
    }
}
