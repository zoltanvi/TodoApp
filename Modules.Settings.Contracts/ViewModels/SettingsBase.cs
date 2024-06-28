using Modules.Common.DataBinding;
using PropertyChanged;
using System.ComponentModel;

namespace Modules.Settings.Contracts.ViewModels;

[AddINotifyPropertyChangedInterface]
public abstract class SettingsBase : IPropertyChangeNotifier
{
    private bool _isDirty;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies about setting property changes.
    /// Listeners can unsubscribe from it without fody interfering.
    /// </summary>
    public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

    /// <summary>
    /// Call this to fire a <see cref="PropertyChanged"/> event
    /// </summary>
    /// <param name="name"></param>
    public void OnPropertyChanged(string name)
    {
        _isDirty = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(name));
    }

    /// <summary>
    /// Returns true if any property had changed since the last clean call.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsDirty() => _isDirty;

    /// <summary>
    /// Resets <see cref="IsDirty"/> to false.
    /// </summary>
    public virtual void Clean() => _isDirty = false;
}

public class SettingsChangedEventArgs(string propertyName) : EventArgs
{
    public string PropertyName { get; } = propertyName;
}