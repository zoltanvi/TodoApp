using Microsoft.Extensions.DependencyInjection;

namespace Modules.Settings.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsRepository(this IServiceCollection services)
    {
        services.AddDbContext<SettingDbContext>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();

        return services;
    }
}
