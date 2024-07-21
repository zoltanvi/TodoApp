using Microsoft.Xaml.Behaviors;
using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using System.Windows;
using System.Windows.Input;
using EventTrigger = Microsoft.Xaml.Behaviors.EventTrigger;

namespace Modules.Common.Views.Services.Navigation;

public class OverlayPageNavigationService : NavigationService, IOverlayPageNavigationService
{
    private UIElement? Background { get; set; }
    private UIElement? Grid { get; set; }
    private ICommand CloseCommand { get; }
    
    public bool PageVisible { get; set; }

    public OverlayPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        CloseCommand = new RelayCommand(ClosePage);
    }

    protected override void BeforeNavigateToPage(Type pageType)
    {
        if (Background == null || Grid == null)
        {
            throw new InvalidOperationException($"{nameof(OverlayPageNavigationService)} is not initialized properly.");
        }

        if (pageType != typeof(IEmptyPage))
        {
            Background.Visibility = Visibility.Visible;
            Grid.Visibility = Visibility.Visible;
            PageVisible = true;
        }
    }

    public void InitializeOverlayElements(object background, object grid)
    {
        Background = background as UIElement;
        Grid = grid as UIElement;

        ArgumentNullException.ThrowIfNull(background);
        ArgumentNullException.ThrowIfNull(grid);

        // Add mouse down trigger to the background that closes the overlay page.
        var eventTrigger = new EventTrigger("MouseDown");
        var invokeCommandAction = new InvokeCommandAction { Command = CloseCommand };
        eventTrigger.Actions.Add(invokeCommandAction);

        Interaction.GetTriggers(Background).Add(eventTrigger);
    }


    private void ClosePage()
    {
        if (Background == null || Grid == null)
        {
            throw new InvalidOperationException($"{nameof(OverlayPageNavigationService)} is not initialized properly.");
        }

        Background.Visibility = Visibility.Collapsed;
        Grid.Visibility = Visibility.Collapsed;
        PageVisible = false;

        NavigateTo<IEmptyPage>();
    }
}
