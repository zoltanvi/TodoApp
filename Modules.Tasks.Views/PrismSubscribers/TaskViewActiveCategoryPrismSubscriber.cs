using Modules.Categories.Contracts.Events;
using Modules.Tasks.Views.Services;
using Prism.Events;

namespace Modules.Tasks.Views.PrismSubscribers;

/// <summary>
/// Task view reactions when active category changes. Subscribes before navigation subscriber (instantiation order in App).
/// </summary>
public sealed class TaskViewActiveCategoryPrismSubscriber
{
    private readonly OneEditorOpenService _oneEditorOpenService;

    public TaskViewActiveCategoryPrismSubscriber(
        IEventAggregator eventAggregator,
        OneEditorOpenService oneEditorOpenService)
    {
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);

        _oneEditorOpenService = oneEditorOpenService;

        eventAggregator.GetEvent<ActiveCategoryChangedEvent>().Subscribe(OnActiveCategoryChanged);
    }

    private void OnActiveCategoryChanged(ActiveCategoryChangedPayload _)
    {
        _oneEditorOpenService.EditModeWithoutTask();
    }
}
