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
            Content = vm.Content,
            ContentPreview = vm.ContentPreview,
            VersionDate = vm.VersionDate
        };
    }

    public static List<TaskItemVersion> MapList(this IEnumerable<TaskItemVersionViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static TaskItemVersionViewModel MapToViewModel(this TaskItemVersion version, IMediator mediator)
    {
        return new TaskItemVersionViewModel(mediator)
        {
            Id = version.Id,
            TaskId = version.TaskId,
            Content = version.Content,
            ContentPreview = version.ContentPreview,
            VersionDate = version.VersionDate
        };
    }

    public static List<TaskItemVersionViewModel> MapToViewModelList(
        this IEnumerable<TaskItemVersion> taskList, 
        IMediator mediator) =>
        taskList.Select(x => x.MapToViewModel(mediator)).ToList();
}
