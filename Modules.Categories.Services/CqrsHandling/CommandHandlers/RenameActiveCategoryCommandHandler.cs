using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Events;
using Modules.PopupMessage.Contracts;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;

namespace Modules.Categories.Services.CqrsHandling.CommandHandlers;

public class RenameActiveCategoryCommandHandler : IRequestHandler<RenameActiveCategoryCommand, string>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IPopupMessageService _popupMessageService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IAppSettings _appSettings;

    public RenameActiveCategoryCommandHandler(
        ICategoriesRepository categoriesRepository,
        IPopupMessageService popupMessageService,
        IEventAggregator eventAggregator,
        IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(popupMessageService);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(appSettings);

        _categoriesRepository = categoriesRepository;
        _popupMessageService = popupMessageService;
        _eventAggregator = eventAggregator;
        _appSettings = appSettings;
    }

    public Task<string> Handle(RenameActiveCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Category name must not be empty!");
        }

        var activeCategoryId = _appSettings.SessionSettings.ActiveCategoryId;
        var category = _categoriesRepository.GetCategoryById(activeCategoryId);

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
                CategoryId = activeCategoryId,
                CategoryName = updatedCategory.Name
            });

        return Task.FromResult(updatedCategory.Name);
    }
}
