using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Events;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
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

        var duplicateCategory = _categoriesRepository.GetCategoryByName(request.Name);

        // TODO: show "do you want to merge it?" message instead
        if (duplicateCategory != null)
        {
            _mediator.Send(new ShowMessageErrorCommand
            {
                Message = $"[{request.Name}] category already exist!"
            }, cancellationToken);

            return Task.FromResult(category.Name);
        }
        
        category.Name = request.Name;

        var updatedCategory = _categoriesRepository.UpdateCategory(category);

        CategoryNameUpdatedEvent.Invoke(new CategoryNameUpdatedEvent
        {
            Id = activeCategoryId,
            CategoryName = updatedCategory.Name
        });

        return Task.FromResult(updatedCategory.Name);
    }
}
