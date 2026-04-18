using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.PopupMessage.Contracts;
using Prism.Events;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RenameCategoryCommandHandler : IRequestHandler<RenameCategoryCommand, string>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IPopupMessageService _popupMessageService;
    private readonly IEventAggregator _eventAggregator;

    public RenameCategoryCommandHandler(
        ICategoriesRepository categoriesRepository,
        IPopupMessageService popupMessageService,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(popupMessageService);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _categoriesRepository = categoriesRepository;
        _popupMessageService = popupMessageService;
        _eventAggregator = eventAggregator;
    }

    public Task<string> Handle(RenameCategoryCommand request, CancellationToken cancellationToken)
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
            _popupMessageService.ShowError($"A sibling category named [{request.Name}] already exists!");

            return Task.FromResult(category.Name);
        }

        category.Name = request.Name;
        var updatedCategory = _categoriesRepository.UpdateCategory(category);

        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Publish(
            new CategoryNameUpdatedPayload
            {
                CategoryId = request.CategoryId,
                CategoryName = updatedCategory.Name
            });

        return Task.FromResult(updatedCategory.Name);
    }
}
