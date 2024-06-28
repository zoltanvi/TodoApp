using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RenameActiveCategoryCommandHandler : IRequestHandler<RenameActiveCategoryCommand, string>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMediator _mediator;

    public RenameActiveCategoryCommandHandler(
        ICategoriesRepository categoriesRepository,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mediator);

        _categoriesRepository = categoriesRepository;
        _mediator = mediator;
    }

    public Task<string> Handle(RenameActiveCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Category name must not be empty!");
        }

        var activeCategoryId = AppSettings.Instance.SessionSettings.ActiveCategoryId;
        var category = _categoriesRepository.GetCategoryById(activeCategoryId);

        ArgumentNullException.ThrowIfNull(category);

        category.Name = request.Name;

        var updatedCategory = _categoriesRepository.UpdateCategory(category);

        var categoryUpdatedEvent = new CategoryNameUpdatedEvent
        {
            Id = activeCategoryId, 
            CategoryName = updatedCategory.Name
        };

        _mediator.Publish(categoryUpdatedEvent, cancellationToken);

        return Task.FromResult(updatedCategory.Name);
    }
}
