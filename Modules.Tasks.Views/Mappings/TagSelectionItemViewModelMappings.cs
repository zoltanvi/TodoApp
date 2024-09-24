using Modules.Common.DataModels;
using Modules.Common.Helpers;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Pages;
using Prism.Events;

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
        IEventAggregator eventAggregator)
    {
        return new TagSelectionItemViewModel(eventAggregator) {
            Id = tagItem.Id,
            Name = tagItem.Name,
            Color = EnumHelper.ConvertTo<TagColor>(tagItem.Color),
        };
    }

    public static List<TagSelectionItemViewModel> MapToViewModelList(this IEnumerable<TagItem> tagList,
        IEventAggregator eventAggregator)
    {
        return tagList.Select(x => x.MapToViewModel(eventAggregator)).ToList();
    }
}
