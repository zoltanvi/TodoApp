using Modules.RecycleBin.Views.Controls;
using Modules.Tasks.Contracts.Models;

namespace Modules.RecycleBin.Views.Mappings;

public static class RecycleBinTaskItemVersionViewModelMappings
{
    public static TaskItemVersion Map(this RecycleBinTaskItemVersionViewModel vm)
    {
        return new TaskItemVersion
        {
            Id = vm.Id,
            TaskId = vm.TaskId,
            Content = vm.Content.GetContent(),
            IsContentPlainText = vm.Content.IsPlainTextMode,
            ContentPreview = vm.Content.GetContentInPlainText(),
            VersionDate = vm.VersionDate
        };
    }

    public static List<TaskItemVersion> MapList(this IEnumerable<RecycleBinTaskItemVersionViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static RecycleBinTaskItemVersionViewModel MapToViewModel(this TaskItemVersion version)
    {
        return new RecycleBinTaskItemVersionViewModel(version.Content, version.IsContentPlainText)
        {
            Id = version.Id,
            TaskId = version.TaskId,
            VersionDate = version.VersionDate
        };
    }

    public static List<RecycleBinTaskItemVersionViewModel> MapToViewModelList(this IEnumerable<TaskItemVersion> taskList) =>
        taskList.Select(x => x.MapToViewModel()).ToList();
}
