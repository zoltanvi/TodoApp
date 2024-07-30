using System.Windows.Input;

namespace Modules.Common.Navigation;

/// <summary>
/// The view models that implement this interface can request the navigation service
/// to close the current page.
/// </summary>
public interface ICloseRequester
{
    ICommand ClosePageCommand { get; set; }
}
