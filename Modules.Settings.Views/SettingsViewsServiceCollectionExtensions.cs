using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Navigation;
using Modules.Settings.Views.Pages;

namespace Modules.Settings.Views;

public static class SettingsViewsServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsPages(this IServiceCollection services)
    {
        services.AddTransient<TaskItemSettingsPageViewModel>();
        services.AddTransient<TagSettingsPageViewModel>();
        services.AddTransient<TaskPageSettingsPageViewModel>();
        services.AddTransient<TaskQuickActionsSettingsPageViewModel>();
        services.AddTransient<TextEditorQuickActionsSettingsPageViewModel>();
        services.AddTransient<ThemeSettingsPageViewModel>();
        services.AddTransient<ApplicationSettingsPageViewModel>();
        services.AddTransient<PageTitleSettingsPageViewModel>();
        services.AddTransient<DateTimeSettingsPageViewModel>();
        services.AddTransient<ShortcutsPageViewModel>();

        services.AddTransient<ITaskItemSettingsPage, TaskItemSettingsPage>();
        services.AddTransient<ITagSettingsPage, TagSettingsPage>();
        services.AddTransient<ITaskPageSettingsPage, TaskPageSettingsPage>();
        services.AddTransient<ITaskQuickActionsSettingsPage, TaskQuickActionsSettingsPage>();
        services.AddTransient<ITextEditorQuickActionsSettingsPage, TextEditorQuickActionsSettingsPage>();
        services.AddTransient<IThemeSettingsPage, ThemeSettingsPage>();
        services.AddTransient<IApplicationSettingsPage, ApplicationSettingsPage>();
        services.AddTransient<IPageTitleSettingsPage, PageTitleSettingsPage>();
        services.AddTransient<IDateTimeSettingsPage, DateTimeSettingsPage>();
        services.AddTransient<IShortcutsPage, ShortcutsPage>();

        return services;
    }
}
