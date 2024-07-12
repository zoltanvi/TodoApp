using Microsoft.Extensions.DependencyInjection;

namespace Modules.RecycleBin.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddRecycleBinRepository(this IServiceCollection services)
    {
        services.AddScoped<RecycleBinRepository>();

        return services;
    }
}
