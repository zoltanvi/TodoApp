namespace Modules.Common.Views.Services;

/// <summary>
/// Use this service locator only in views, where it's not possible to inject the provider into the ctor!
/// </summary>
public static class ServiceLocator
{
    public static IServiceProvider ServiceProvider { get; set; }

    public static T? GetService<T>() where T : class
    {
        return ServiceProvider.GetService(typeof(T)) as T;
    }
}
