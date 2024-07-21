using Modules.Common.Services.Navigation;
using System.Windows;

namespace Modules.Common.Views.Services.Navigation;

public class OverlayPageNavigationService : NavigationService, IOverlayPageNavigationService
{
    private UIElement? Background { get; set; }
    private UIElement? Grid { get; set; }

    public OverlayPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void BeforeNavigateToPage()
    {
        if (Background == null || Grid == null)
        {
            throw new InvalidOperationException($"{nameof(OverlayPageNavigationService)} is not initialized with a Border as Background.");
        }

        Background.Visibility = Visibility.Visible;
        Grid.Visibility = Visibility.Visible;
    }

    public void InitializeOverlayElements(object background, object grid)
    {
        Background = background as UIElement;
        Grid = grid as UIElement;
    }
}
