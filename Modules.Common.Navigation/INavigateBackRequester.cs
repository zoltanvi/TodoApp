using System.Windows.Input;

namespace Modules.Common.Navigation;

/// <summary>
/// The view models that implement this interface can request the navigation service
/// to navigate back to the previous page.
/// </summary>
public interface INavigateBackRequester
{
    ICommand NavigateBackCommand { get; set; }
}
