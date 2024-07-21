namespace Modules.Common.Services.Navigation;

public interface IOverlayPageNavigationService : INavigationService
{
    void InitializeOverlayElements(object background, object grid);
    
    /// <summary>
    /// True if the overlay page is currently visible
    /// </summary>
    public bool PageVisible { get; set; }
}
