using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.PopupMessage.Contracts.Cqrs.Commands;
using Prism.Events;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class MoveCategoryCommandHandler : IRequestHandler<MoveCategoryCommand>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMediator _mediator;
    private readonly IEventAggregator _eventAggregator;

    public MoveCategoryCommandHandler(
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

    public async Task Handle(MoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _categoriesRepository.GetCategoryById(request.CategoryId);
        ArgumentNullException.ThrowIfNull(category);

        if (request.NewParentCategoryId == request.CategoryId)
        {
            await _mediator.Send(new ShowMessageErrorCommand
            {
                Message = "Cannot move a category under itself!"
            }, cancellationToken);
            return;
        }

        // Prevent circular references
        if (request.NewParentCategoryId.HasValue)
        {
            var descendants = _categoriesRepository.GetDescendantCategoryIds(request.CategoryId);
            if (descendants.Contains(request.NewParentCategoryId.Value))
            {
                await _mediator.Send(new ShowMessageErrorCommand
                {
                    Message = "Cannot move a category under its own descendant!"
                }, cancellationToken);
                return;
            }
        }

        var oldParentId = category.ParentCategoryId;

        // Determine new list order: append at end of new parent's children
        int newListOrder;
        if (request.NewParentCategoryId.HasValue)
        {
            var siblings = _categoriesRepository.GetChildCategories(request.NewParentCategoryId.Value);
            newListOrder = siblings.Count > 0 ? siblings.Max(x => x.ListOrder) + 1 : 0;
        }
        else
        {
            var rootCategories = _categoriesRepository.GetRootCategories();
            newListOrder = rootCategories.Count > 0 ? rootCategories.Max(x => x.ListOrder) + 1 : 0;
        }

        _categoriesRepository.MoveCategoryToParent(request.CategoryId, request.NewParentCategoryId, newListOrder);

        _eventAggregator.GetEvent<CategoryMovedEvent>().Publish(new CategoryMovedPayload
        {
            CategoryId = request.CategoryId,
            OldParentCategoryId = oldParentId,
            NewParentCategoryId = request.NewParentCategoryId
        });
    }
}
