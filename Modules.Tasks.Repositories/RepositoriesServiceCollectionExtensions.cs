using Microsoft.Extensions.DependencyInjection;
using Modules.Tasks.Contracts;

namespace Modules.Tasks.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddTaskItemRepository(this IServiceCollection services)
    {
        services.AddDbContext<TaskItemDbContext>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return services;
    }
}
