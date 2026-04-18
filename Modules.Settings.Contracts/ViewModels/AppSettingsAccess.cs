namespace Modules.Settings.Contracts.ViewModels;

/// <summary>
/// Bridge for WPF types that cannot use constructor injection (e.g. value converters, some control code-behind).
/// Set once at startup from <see cref="IAppSettings"/> resolved from DI.
/// </summary>
public static class AppSettingsAccess
{
    public static IAppSettings? Current { get; set; }
}
