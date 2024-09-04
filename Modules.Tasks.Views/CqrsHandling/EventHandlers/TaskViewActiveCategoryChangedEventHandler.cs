using MediatR;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Tasks.Views.Services;

namespace Modules.Tasks.Views.CqrsHandling.EventHandlers;

public class TaskViewActiveCategoryChangedEventHandler : INotificationHandler<ActiveCategoryChangedEvent>
{
    private readonly OneEditorOpenService _oneEditorOpenService;

    public TaskViewActiveCategoryChangedEventHandler(OneEditorOpenService oneEditorOpenService)
    {
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);
        _oneEditorOpenService = oneEditorOpenService;
    }

    public Task Handle(ActiveCategoryChangedEvent notification, CancellationToken cancellationToken)
    {
        _oneEditorOpenService.EditModeWithoutTask();
        return Task.CompletedTask;
    }
}
