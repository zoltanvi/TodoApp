using Microsoft.Extensions.DependencyInjection;

namespace Modules.Migration;

public static class MigrationServiceCollectionExtensions
{
    public static IServiceCollection AddMigrationsService(this IServiceCollection services)
    {
        services.AddScoped<IMigrationService, MigrationService>();

        return services;
    }
}
