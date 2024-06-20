using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Services;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Services;
using Modules.Common.Views.Services.Navigation;
using Modules.Migration;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Services.CqrsHandling;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;
using TodoApp.Themes;
using TodoApp.WindowHandling;

namespace TodoApp;

public static class Program
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<App>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ServicesMediatorRegistration>());

        services.AddSingleton<IUIScaler>(provider => UIScaler.Instance);
        services.AddSingleton<MaterialThemeManagerService>();
        services.AddSingleton<ThemeManager>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddScoped<MainWindow>();
        services.AddScoped<MainWindowViewModel>();

        services.AddSingleton<AppSettings>(provider => AppSettings.Instance);
        services.AddScoped<IAppSettingsService, AppSettingsService>();

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
