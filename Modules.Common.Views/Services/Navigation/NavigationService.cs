﻿using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using System.Windows.Controls;

namespace Modules.Common.Views.Services.Navigation;

public abstract class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private Frame? Frame { get; set; }

    protected NavigationService(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public void Initialize(object frame)
    {
        Frame = frame as Frame;
    }

    public void NavigateTo<T>() where T : class, IPage
    {
        if (Frame == null)
        {
            throw new InvalidOperationException($"{nameof(NavigationService)} is not initialized with a Frame.");
        }

        if (Frame.Content is Page oldPage &&
            oldPage?.DataContext is IDisposable disposableDataContext)
        {
            disposableDataContext.Dispose();
        }

        var page = GetPage<T>();
        if (page != null)
        {
            Frame.Navigate(page);
        }
        else
        {
            throw new InvalidOperationException($"No page registered for {typeof(T).Name}");
        }
    }

    protected virtual Page? GetPage<T>() where T : class, IPage
    {
        return GetPageFromServices<T>();
    }

    protected Page? GetPageFromServices<T>() where T : class, IPage
    {
        return _serviceProvider.GetService(typeof(T)) as Page;
    }
}
