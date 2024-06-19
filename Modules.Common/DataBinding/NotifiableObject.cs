namespace Modules.Common.DataBinding;

public class NotifiableObject(Action? notifyAction) : INotifiableObject
{
    public void Notify() => notifyAction?.Invoke();
}
