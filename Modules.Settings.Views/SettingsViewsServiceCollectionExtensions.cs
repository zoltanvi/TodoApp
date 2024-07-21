using Microsoft.Extensions.DependencyInjection;
using Modules.Settings.Views.Pages;

namespace Modules.Settings.Views;

public static class SettingsViewsServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsViews(this IServiceCollection services)
    {
        services.AddTransient<NotePageSettingsPageViewModel>();
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

        services.AddTransient<NotePageSettingsPage>();
        services.AddTransient<TaskItemSettingsPage>();
        services.AddTransient<TagSettingsPage>();
        services.AddTransient<TaskPageSettingsPage>();
        services.AddTransient<TaskQuickActionsSettingsPage>();
        services.AddTransient<TextEditorQuickActionsSettingsPage>();
        services.AddTransient<ThemeSettingsPage>();
        services.AddTransient<ApplicationSettingsPage>();
        services.AddTransient<PageTitleSettingsPage>();
        services.AddTransient<DateTimeSettingsPage>();
        services.AddTransient<ShortcutsPage>();

        return services;
    }
}
