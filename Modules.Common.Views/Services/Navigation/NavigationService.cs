using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using System.Windows.Controls;

namespace Modules.Common.Views.Services.Navigation;

public abstract class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private Frame? Frame { get; set; }
    private Type? PreviousPageType { get; set; }
    private Type? CurrentPageType { get; set; }
    private Type? NextPageType { get; set; }

    protected NavigationService(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public void Initialize(object frame)
    {
        Frame = frame as Frame;
    }

    public void NavigateTo<T>() where T : class, IPage => NavigateTo(typeof(T));

    public bool GoBackToPreviousPage()
    {
        if (PreviousPageType == null) return false;

        NavigateTo(PreviousPageType);

        PreviousPageType = null;

        return true;
    }

    public bool GoToNextPage()
    {
        if (NextPageType == null) return false;

        NavigateTo(NextPageType);

        NextPageType = null;

        return true;
    }

    private void NavigateTo(Type pageType)
    {
        if (Frame == null)
        {
            throw new InvalidOperationException($"{nameof(NavigationService)} is not initialized with a Frame.");
        }

        if (!typeof(IPage).IsAssignableFrom(pageType))
        {
            throw new ArgumentException("The type must be a class and implement IPage.", nameof(pageType));
        }

        if (Frame.Content is Page oldPage &&
            oldPage.DataContext is IDisposable disposableDataContext)
        {
            disposableDataContext.Dispose();
        }

        if (_serviceProvider.GetService(pageType) is Page page)
        {
            if (CurrentPageType != null)
            {
                PreviousPageType = CurrentPageType;
            }

            CurrentPageType = pageType;

            Frame.Navigate(page);
        }
        else
        {
            throw new InvalidOperationException($"No page registered for {pageType.Name}");
        }
    }
}
