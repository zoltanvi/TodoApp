using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Categories.Repositories;
using Modules.Categories.Services.CqrsHandling;
using Modules.Categories.Views.Pages;
using Modules.Common.Database;
using Modules.Common.Navigation;
using Modules.Common.Services;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Pages;
using Modules.Common.Views.Services;
using Modules.Common.Views.Services.Navigation;
using Modules.Migration;
using Modules.PopupMessage.Views;
using Modules.PopupMessage.Views.CqrsHandling;
using Modules.RecycleBin.Repositories;
using Modules.RecycleBin.Views.Pages;
using Modules.Settings.Contracts;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Services.CqrsHandling;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;
using Modules.Settings.Views.Services;
using Modules.Tasks.Repositories;
using Modules.Tasks.Views.CqrsHandling;
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
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<App>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SettingsCqrsRegistration>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PopupMessageCqrsRegistration>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CategoriesCqrsRegistration>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<TasksCqrsRegistration>());

        // Prism.Core
        services.AddSingleton<IEventAggregator, EventAggregator>();

        services.AddSingleton<IUIScaler>(provider =>
        {
            var mediator = provider.GetRequiredService<IMediator>();
            var eventAggregator = provider.GetRequiredService<IEventAggregator>();
            UIScaler.Instance.Setup(mediator, eventAggregator);

            return UIScaler.Instance;
        });

        services.AddSingleton<IThemeEditorService, ThemeEditorService>();
        services.AddSingleton<MaterialThemeManagerService>();
        services.AddSingleton<ThemeManager>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddScoped<MainWindow>();
        services.AddScoped<MainWindowViewModel>();

        services.AddSingleton<OneEditorOpenService>(provider => OneEditorOpenService.Instance);
        services.AddSingleton<AppSettings>(provider => AppSettings.Instance);
        services.AddSingleton<IAppSettingsAutoSaveService, AppSettingsAutoSaveService>();
        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>();
        services.AddSingleton<ISideMenuPageNavigationService, SideMenuPageNavigationService>();
        services.AddSingleton<IOverlayPageNavigationService, OverlayPageNavigationService>();
        services.AddSingleton<PopupMessageControl>();

        services.AddScoped<IAppSettingsService, AppSettingsService>();
        
        services.AddSettingsViews();

        AddDatabases(services);
        AddPages(services);

        return services;
    }

    public static void InitializeDatabase(this IServiceProvider serviceProvider)
    {
        DbConfiguration.Initialize(serviceProvider.GetRequiredService<IConfiguration>());

        var migrationService = serviceProvider.GetRequiredService<IMigrationService>();

        var dbContextList = new List<DbContext>
        {
            serviceProvider.GetRequiredService<SettingDbContext>(),
            serviceProvider.GetRequiredService<CategoryDbContext>(),
            serviceProvider.GetRequiredService<TaskItemDbContext>()
        };

        migrationService.Run(dbContextList);

        serviceProvider.CreateDefaultData();
    }

    private static void CreateDefaultData(this IServiceProvider serviceProvider)
    {
        var defaultDataCreator = serviceProvider.GetRequiredService<DefaultDataCreator>();

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
        services.AddTransient<TaskPageViewModel>();

        services.AddTransient<IRecycleBinPage, RecycleBinPage>();
        services.AddTransient<RecycleBinPageViewModel>();

        services.AddTransient<ITagSelectorPage, TagSelectorPage>();
        services.AddTransient<TagSelectorPageViewModel>();

        services.AddTransient<ITaskHistoryPage, TaskHistoryPage>();
        services.AddTransient<TaskHistoryPageViewModel>();

        services.AddSingleton<IEmptyPage, EmptyPage>();
        
        //services.AddScoped<INoteEditorPage, NotePage>();
        //services.AddScoped<INoteListPage, NoteListPage>();
        
        //services.AddScoped<ITaskNotificationPage, NotificationPage>();
        //services.AddScoped<ITaskReminderEditorPage, ReminderEditorPage>();
        //services.AddScoped<ITaskReminderPage, TaskReminderPage>();

        //services.AddScoped<NotePageViewModel>();
        //services.AddScoped<NoteListPageViewModel>();

        //services.AddScoped<NotificationPageViewModel>();
        //services.AddScoped<ReminderEditorPageViewModel>();
        //services.AddScoped<TaskReminderPageViewModel>();
    }
}
