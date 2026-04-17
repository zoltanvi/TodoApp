using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.Categories.Contracts.Models;
using Prism.Events;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RestoreCategoryCommandHandler : IRequestHandler<RestoreCategoryCommand>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IEventAggregator _eventAggregator;

    public RestoreCategoryCommandHandler(
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _categoriesRepository = categoriesRepository;
        _eventAggregator = eventAggregator;
    }

    public Task Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        var dbCategory = _categoriesRepository.GetCategoryById(request.Id);
        ArgumentNullException.ThrowIfNull(dbCategory);

        // Collect deleted ancestors so the full hierarchy path is restored
        var ancestorsToRestore = new List<Category>();
        CollectDeletedAncestors(dbCategory, ancestorsToRestore);

        // Restore ancestors from top-most parent down to direct parent
        foreach (var ancestor in ancestorsToRestore)
        {
            var activeCategoriesCount = _categoriesRepository.GetActiveCategoriesCount();
            _categoriesRepository.RestoreCategory(ancestor, activeCategoriesCount);
            _eventAggregator.GetEvent<CategoryRestoredEvent>().Publish(ancestor.Id);
        }

        // Restore the requested category itself
        var activeCount = _categoriesRepository.GetActiveCategoriesCount();
        _categoriesRepository.RestoreCategory(dbCategory, activeCount);
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Publish(dbCategory.Id);

        return Task.CompletedTask;
    }

    private void CollectDeletedAncestors(Category category, List<Category> ancestors)
    {
        if (!category.ParentCategoryId.HasValue) return;

        var parent = _categoriesRepository.GetCategoryById(category.ParentCategoryId.Value);
        if (parent is not { IsDeleted: true }) return;

        CollectDeletedAncestors(parent, ancestors);
        ancestors.Add(parent);
    }
}
