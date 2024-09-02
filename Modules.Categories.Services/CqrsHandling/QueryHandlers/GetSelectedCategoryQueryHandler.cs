using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Categories.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.CqrsHandling.QueryHandlers;

public class GetSelectedCategoryQueryHandler : IRequestHandler<GetSelectedCategoryQuery, CategoryInfo>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public GetSelectedCategoryQueryHandler(ICategoriesRepository categoriesRepository)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        _categoriesRepository = categoriesRepository;
    }

    public Task<CategoryInfo> Handle(GetSelectedCategoryQuery request, CancellationToken cancellationToken)
    {
        var id = AppSettings.Instance.SessionSettings.ActiveCategoryId;
        Category? activeCategory = _categoriesRepository.GetCategoryById(id);

        return Task.FromResult(new CategoryInfo
        {
            Id = activeCategory?.Id ?? -1,
            Name = activeCategory?.Name ?? string.Empty
        });
    }
}