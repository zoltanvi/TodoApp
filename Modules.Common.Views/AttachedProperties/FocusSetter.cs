using System.Windows;

namespace Modules.Common.Views.AttachedProperties;

/// <summary>
/// The <see cref="FocusSetter"/> attached property for setting focus on a <see cref="UIElement"/> when requested
/// </summary>
public class FocusSetter : BaseAttachedProperty<FocusSetter, bool>
{
    public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        bool oldValue = (bool)e.OldValue;
        bool newValue = (bool)e.NewValue;

        if (sender is UIElement uiElement && !oldValue && newValue)
        {
            uiElement.Focus();
        }
    }
}