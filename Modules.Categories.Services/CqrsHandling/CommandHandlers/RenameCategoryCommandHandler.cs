using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Prism.Events;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RenameCategoryCommandHandler : IRequestHandler<RenameCategoryCommand, string>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public RenameCategoryCommandHandler(
        ICategoriesRepository categoriesRepository,
        IMediator mediator,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _categoriesRepository = categoriesRepository;
        _mediator = mediator;
        _eventAggregator = eventAggregator;
    }

    public async Task<string> Handle(RenameCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Category name must not be empty!");
        }

        var category = _categoriesRepository.GetCategoryById(request.CategoryId);
        ArgumentNullException.ThrowIfNull(category);

        var duplicateCategory = _categoriesRepository.GetCategoryByName(request.Name, category.ParentCategoryId);

        if (duplicateCategory != null && duplicateCategory.Id != category.Id)
        {
            await _mediator.Send(new ShowMessageErrorCommand
            {
                Message = $"A sibling category named [{request.Name}] already exists!"
            }, cancellationToken);

            return category.Name;
        }

        category.Name = request.Name;
        var updatedCategory = _categoriesRepository.UpdateCategory(category);

        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Publish(
            new CategoryNameUpdatedPayload
            {
                CategoryId = request.CategoryId,
                CategoryName = updatedCategory.Name
            });

        return updatedCategory.Name;
    }
}
