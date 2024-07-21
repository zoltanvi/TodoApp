namespace Modules.Common.Services.Navigation;

public interface IOverlayPageNavigationService : INavigationService
{
    void InitializeOverlayElements(object background, object grid);
}
