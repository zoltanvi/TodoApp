using Microsoft.Extensions.DependencyInjection;
using Modules.Categories.Contracts;

namespace Modules.Categories.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddCategoriesRepository(this IServiceCollection services)
    {
        services.AddDbContext<CategoryDbContext>();
        services.AddScoped<ICategoriesDbInfoRepository, CategoriesDbInfoRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();

        return services;
    }
}
