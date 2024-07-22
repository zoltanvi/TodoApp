namespace Modules.Common.Navigation;

/// <summary>
/// Interface to pass parameters to ViewModels of IPages
/// </summary>
public interface IParameterReceiver
{
    void ReceiveParameter(object parameter);
}
