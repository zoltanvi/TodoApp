using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
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

        var activeCategoriesCount = _categoriesRepository.GetActiveCategoriesCount();

        _categoriesRepository.RestoreCategory(dbCategory, activeCategoriesCount);

        // Notify view
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Publish(dbCategory.Id);

        return Task.CompletedTask;
    }
}
