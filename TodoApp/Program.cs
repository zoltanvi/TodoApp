using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Services;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Services;
using Modules.Common.Views.Services.Navigation;
using Modules.Migration;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;

namespace TodoApp;

public static class Program
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<App>());

        services.AddScoped<MainWindow>();
        services.AddScoped<IUIScaler, UIScaler>();

        services.AddSingleton<AppSettings>(provider => AppSettings.Instance);
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<UIScaler>();
        services.AddSingleton<IUIScaler, UIScaler>();

        services.AddScoped<SettingsPage>();
        services.AddScoped<SettingsPageViewModel>();
        services.AddSettingsViews();

        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>();
        services.AddSingleton<ISideMenuPageNavigationService, SideMenuPageNavigationService>();
        services.AddSingleton<IOverlayPageNavigationService, OverlayPageNavigationService>();

        AddDatabases(services);

        return services;
    }

    private static void AddDatabases(IServiceCollection services)
    {
        services.AddSettingsRepository();
        
        services.AddMigrationsService();
    }
}
