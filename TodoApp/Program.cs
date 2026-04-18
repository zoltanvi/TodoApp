using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Categories.Repositories;
using Modules.Categories.Services.CqrsHandling.CommandHandlers;
using Modules.Categories.Views.DragDrop;
using Modules.Categories.Views.Pages;
using Modules.Common.Database;
using Modules.Common.Navigation;
using Modules.Common.Services;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Pages;
using Modules.Common.Views.Services;
using Modules.Common.Views.Services.Navigation;
using Modules.Migration;
using Modules.PopupMessage.Contracts;
using Modules.PopupMessage.Views;
using Modules.RecycleBin.Repositories;
using Modules.RecycleBin.Views.Pages;
using Modules.Settings.Contracts;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Services.PrismSubscribers;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;
using Modules.Settings.Views.Services;
using Modules.Tasks.Repositories;
using Modules.Tasks.Services.CqrsHandling.CommandHandlers;
using Modules.Tasks.Views.Pages;
using Modules.Tasks.Views.Services;
using Prism.Events;
using TodoApp.DefaultData;
using TodoApp.Themes;
using TodoApp.WindowHandling;

namespace TodoApp;

public static class Program
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
    {
        AddMediatR(services);

        // Messaging split (keep both; different roles):
        // - Prism IEventAggregator: UI/cross-component pub-sub (clicks, lifecycle, theme, active category, task list reactions).
        // - MediatR: IRequest handlers for application operations (repos, multi-step workflows, nested Send). Prefer direct
        //   I*NavigationService.NavigateTo from viewmodels when the only work is opening a page (no MediatR indirection).
        services.AddSingleton<IEventAggregator, EventAggregator>();

        services.AddSingleton<IAppSettings>(sp => AppSettings.Instance);
        services.AddSingleton<PopupMessageManager>();
        services.AddSingleton<IPopupMessageService, PopupMessageService>();

        services.AddSingleton<IUIScaler>(provider =>
        {
            var uiScaler = UIScaler.Instance;
            var popupMessageService = provider.GetRequiredService<IPopupMessageService>();
            var eventAggregator = provider.GetRequiredService<IEventAggregator>();
            uiScaler.Setup(popupMessageService, eventAggregator);

            return uiScaler;
        });

        services.AddSingleton<ApplicationLifecyclePrismSubscriber>();
        services.AddSingleton<ActiveCategoryChangedCoordinator>();

        services.AddSingleton<TaskToCategoryDropHandler>();

        services.AddSingleton<IThemeEditorService, ThemeEditorService>();
        services.AddSingleton<MaterialThemeManagerService>();
        services.AddSingleton<ThemeManager>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddScoped<MainWindow>();
        services.AddScoped<MainWindowViewModel>();

        services.AddSingleton<OneEditorOpenService>(provider => OneEditorOpenService.Instance);
        services.AddSingleton<AppSettings>(provider => AppSettings.Instance);
        services.AddSingleton<IAppSettingsAutoSaveService, AppSettingsAutoSaveService>();
        services.AddSingleton<PopupMessageControl>(sp => new PopupMessageControl(sp.GetRequiredService<PopupMessageManager>()));

        services.AddScoped<IAppSettingsService, AppSettingsService>();

        AddDatabases(services);
        AddNavigation(services);
        AddPages(services);

        return services;
    }

    private static void AddMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(MoveCategoryCommandHandler).Assembly,
            typeof(UpdateTaskCommandHandler).Assembly));
    }

    public static void InitializeDatabase(this IServiceProvider serviceProvider)
    {
        DbConfiguration.Initialize(serviceProvider.GetRequiredService<IConfiguration>());

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var taskItemContext = scopedProvider.GetRequiredService<TaskItemDbContext>();

        var migrationService = scopedProvider.GetRequiredService<IMigrationService>();

        var dbContextList = new List<DbContext>
        {
            scopedProvider.GetRequiredService<SettingDbContext>(),
            scopedProvider.GetRequiredService<CategoryDbContext>(),
            taskItemContext
        };

        migrationService.Run(dbContextList);

        // Create default data
        var defaultDataCreator = scopedProvider.GetRequiredService<DefaultDataCreator>();
        defaultDataCreator.CreateDefaultsIfNeeded();
    }

    private static void AddDatabases(IServiceCollection services)
    {
        services.AddScoped<DefaultDataCreator>();

        services.AddSettingsRepository();
        services.AddCategoriesRepository();
        services.AddTaskItemRepository();
        services.AddRecycleBinRepository();
        
        services.AddMigrationsService();
    }

    private static void AddPages(IServiceCollection services)
    {
        services.AddSingleton<ISettingsPage, SettingsPage>();
        services.AddSingleton<SettingsPageViewModel>();
        
        services.AddTransient<ICategoryListPage, CategoryPage>();
        services.AddTransient<CategoryPageViewModel>();

        services.AddTransient<ITaskPage, TaskPage>();
        services.AddTransient<TaskDragDropIndexModifier>();
        services.AddTransient<TaskPageViewModel>();

        services.AddTransient<IRecycleBinPage, RecycleBinPage>();
        services.AddTransient<RecycleBinPageViewModel>();

        services.AddTransient<ITagSelectorPage, TagSelectorPage>();
        services.AddTransient<TagSelectorPageViewModel>();

        services.AddTransient<ITaskHistoryPage, TaskHistoryPage>();
        services.AddTransient<TaskHistoryPageViewModel>();

        services.AddSingleton<IEmptyPage, EmptyPage>();

        // Settings pages
        services.AddSettingsPages();
    }

    private static void AddNavigation(IServiceCollection services)
    {
        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>();
        services.AddSingleton<ISideMenuPageNavigationService, SideMenuPageNavigationService>();
        services.AddSingleton<IOverlayPageNavigationService, OverlayPageNavigationService>();
        services.AddSingleton<ISettingsPageNavigationService, SettingsPageNavigationService>();
    }
}
