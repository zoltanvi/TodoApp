using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Categories.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.CqrsHandling.QueryHandlers;

public class GetInactiveCategoriesQueryHandler : IRequestHandler<GetInactiveCategoriesQuery, List<CategoryInfo>>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IAppSettings _appSettings;

    public GetInactiveCategoriesQueryHandler(ICategoriesRepository categoriesRepository, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(appSettings);
        _categoriesRepository = categoriesRepository;
        _appSettings = appSettings;
    }

    public Task<List<CategoryInfo>> Handle(GetInactiveCategoriesQuery request, CancellationToken cancellationToken)
    {
        var activeCategoryId = _appSettings.SessionSettings.ActiveCategoryId;
        var inactiveCategories = _categoriesRepository.GetActiveCategories().Where(x => x.Id != activeCategoryId);

        var result = new List<CategoryInfo>(
            inactiveCategories.Select(x => new CategoryInfo
            {
                Id = x.Id,
                ParentCategoryId = x.ParentCategoryId,
                Name = x.Name
            }));

        return Task.FromResult(result);
    }
}
