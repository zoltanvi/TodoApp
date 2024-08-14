using MediatR;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Views.Controls;

namespace Modules.Tasks.Views.Mappings;

public static class TaskItemVersionViewModelMappings
{
    public static TaskItemVersion Map(this TaskItemVersionViewModel vm)
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

    public static List<TaskItemVersion> MapList(this IEnumerable<TaskItemVersionViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TaskItemVersionViewModel MapToViewModel(this TaskItemVersion version, IMediator mediator)
    {
        return new TaskItemVersionViewModel(
            mediator, 
            version.IsContentPlainText, 
            version.Content)
        {
            Id = version.Id,
            TaskId = version.TaskId,
            VersionDate = version.VersionDate
        };
    }

    public static List<TaskItemVersionViewModel> MapToViewModelList(
        this IEnumerable<TaskItemVersion> taskList, 
        IMediator mediator) =>
        taskList.Select(x => x.MapToViewModel(mediator)).ToList();
}
