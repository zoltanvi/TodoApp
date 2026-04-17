using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Events;
using Prism.Events;

namespace Modules.Tasks.Services.CqrsHandling.CommandHandlers;

public class RestoreCategoryTreeCommandHandler : IRequestHandler<RestoreCategoryTreeCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public RestoreCategoryTreeCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _taskItemRepository = taskItemRepository;
        _categoriesRepository = categoriesRepository;
        _mediator = mediator;
        _eventAggregator = eventAggregator;
    }

    public async Task Handle(RestoreCategoryTreeCommand request, CancellationToken cancellationToken)
    {
        var rootCategory = _categoriesRepository.GetCategoryById(request.RootCategoryId);
        ArgumentNullException.ThrowIfNull(rootCategory);

        var descendantIds = _categoriesRepository.GetDeletedDescendantCategoryIds(request.RootCategoryId);
        var allCategoryIds = new List<int> { request.RootCategoryId };
        allCategoryIds.AddRange(descendantIds);

        // Restore categories from root to leaves so the sidebar builds the tree correctly
        foreach (var categoryId in allCategoryIds)
        {
            var category = _categoriesRepository.GetCategoryById(categoryId);
            if (category is { IsDeleted: true })
            {
                await _mediator.Send(new RestoreCategoryCommand { Id = category.Id }, cancellationToken);
            }
        }

        // Restore tasks for each category
        foreach (var categoryId in allCategoryIds)
        {
            var deletedTasks = _taskItemRepository.GetDeletedTasksFromCategory(categoryId);
            if (deletedTasks.Count > 0)
            {
                _taskItemRepository.RestoreTasksInCategory(categoryId, 0);
            }
        }

        _eventAggregator.GetEvent<CategoryTreeRestoredEvent>().Publish(request.RootCategoryId);
    }
}
