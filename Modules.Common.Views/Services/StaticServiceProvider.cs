namespace Modules.Common.Views.Services;

/// <summary>
/// Allows UI elements that are not created with dependency injection to get a serviceProvider.
/// </summary>
public static class StaticServiceProvider
{
    public static IServiceProvider? ServiceProvider { get; set; }
}
