using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Categories.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.CqrsHandling.QueryHandlers;

public class GetSelectedCategoryQueryHandler : IRequestHandler<GetSelectedCategoryQuery, CategoryInfo>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IAppSettings _appSettings;

    public GetSelectedCategoryQueryHandler(ICategoriesRepository categoriesRepository, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(appSettings);
        _categoriesRepository = categoriesRepository;
        _appSettings = appSettings;
    }

    public Task<CategoryInfo> Handle(GetSelectedCategoryQuery request, CancellationToken cancellationToken)
    {
        var id = _appSettings.SessionSettings.ActiveCategoryId;
        Category? activeCategory = _categoriesRepository.GetCategoryById(id);

        return Task.FromResult(new CategoryInfo
        {
            Id = activeCategory?.Id ?? -1,
            ParentCategoryId = activeCategory?.ParentCategoryId,
            Name = activeCategory?.Name ?? string.Empty
        });
    }
}