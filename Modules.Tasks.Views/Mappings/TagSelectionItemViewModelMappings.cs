using Modules.Common.DataModels;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Pages;

namespace Modules.Tasks.Views.Mappings;

public static class TagSelectionItemViewModelMappings
{
    public static TagItem Map(this TagSelectionItemViewModel vm)
    {
        return new TagItem
        {
            Id = vm.Id,
            Name = vm.Name,
            Color = vm.Color.ToString(),
        };
    }

    public static List<TagItem> MapList(this IEnumerable<TagSelectionItemViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TagSelectionItemViewModel MapToViewModel(this TagItem tagItem,
        TagSelectorPageViewModel tagSelectorPageViewModel)
    {
        return new TagSelectionItemViewModel (tagSelectorPageViewModel) {
            Id = tagItem.Id,
            Name = tagItem.Name,
            Color = (TagPresetColor)Enum.Parse(typeof(TagPresetColor), tagItem.Color),
        };
    }

    public static List<TagSelectionItemViewModel> MapToViewModelList(this IEnumerable<TagItem> tagList,
        TagSelectorPageViewModel tagSelectorPageViewModel)
    {
        return tagList.Select(x => x.MapToViewModel(tagSelectorPageViewModel)).ToList();
    }
}
