using Modules.Common.Navigation;

namespace Modules.Common.Services.Navigation;

public interface INavigationService
{
    void Initialize(object frame);
    void NavigateTo<T>(object? parameter = null) where T : class, IPage;
    bool GoBackToPreviousPage();
    public bool GoToNextPage();
}