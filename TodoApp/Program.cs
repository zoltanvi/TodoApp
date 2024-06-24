﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Categories.Repositories;
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
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Services.CqrsHandling;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;
using Modules.Settings.Views.Services;
using TodoApp.DefaultData;
using TodoApp.Themes;
using TodoApp.WindowHandling;

namespace TodoApp;

public static class Program
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<App>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SettingsCqrsRegistration>());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PopupMessageCqrsRegistration>());

        services.AddSingleton<IUIScaler>(provider => UIScaler.Instance);
        services.AddSingleton<MaterialThemeManagerService>();
        services.AddSingleton<ThemeManager>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddScoped<MainWindow>();
        services.AddScoped<MainWindowViewModel>();

        services.AddSingleton<AppSettings>(provider => AppSettings.Instance);
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<IAppSettingsAutoSaveService, AppSettingsAutoSaveService>();

        services.AddScoped<SettingsPage>();
        services.AddScoped<SettingsPageViewModel>();
        services.AddSettingsViews();

        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>();
        services.AddSingleton<ISideMenuPageNavigationService, SideMenuPageNavigationService>();
        services.AddSingleton<IOverlayPageNavigationService, OverlayPageNavigationService>();

        services.AddSingleton<PopupMessageControl>();

        AddDatabases(services);
        AddPages(services);

        return services;
    }

    public static void InitializeDatabase(this IServiceProvider serviceProvider)
    {
        DbConfiguration.Initialize(serviceProvider.GetRequiredService<IConfiguration>());

        var migrationService = serviceProvider.GetService<IMigrationService>();

        var dbContextList = new List<DbContext>
        {
            serviceProvider.GetService<SettingDbContext>(),
            serviceProvider.GetService<CategoryDbContext>(),
        };

        migrationService.Run(dbContextList);

        serviceProvider.CreateDefaultData();
    }

    private static void CreateDefaultData(this IServiceProvider serviceProvider)
    {
        var defaultDataCreator = serviceProvider.GetService<DefaultDataCreator>();

        defaultDataCreator.CreateDefaultsIfNeeded();
    }

    private static void AddDatabases(IServiceCollection services)
    {
        services.AddScoped<DefaultDataCreator>();

        services.AddSettingsRepository();
        services.AddCategoriesRepository();
        
        services.AddMigrationsService();
    }

    private static void AddPages(IServiceCollection services)
    {
        services.AddScoped<ISettingsPage, SettingsPage>();
        services.AddScoped<SettingsPageViewModel>();
        
        services.AddScoped<ICategoryListPage, CategoryListPage>();
        services.AddScoped<CategoryListPageViewModel>();


        //services.AddScoped<INoteEditorPage, NotePage>();
        //services.AddScoped<INoteListPage, NoteListPage>();
        //services.AddScoped<IRecycleBinPage, RecycleBinPage>();
        //services.AddSingleton<ITaskPage, TaskPage>();

        services.AddScoped<IEmptyPage, EmptyPage>();
        //services.AddScoped<ITaskNotificationPage, NotificationPage>();
        //services.AddScoped<ITaskReminderEditorPage, ReminderEditorPage>();
        //services.AddScoped<ITaskReminderPage, TaskReminderPage>();

        //services.AddScoped<NotePageViewModel>();
        //services.AddScoped<NoteListPageViewModel>();
        //services.AddScoped<RecycleBinPageViewModel>();
        //services.AddScoped<TaskPageViewModel>();

        //services.AddScoped<NotificationPageViewModel>();
        //services.AddScoped<ReminderEditorPageViewModel>();
        //services.AddScoped<TaskReminderPageViewModel>();
    }
}
