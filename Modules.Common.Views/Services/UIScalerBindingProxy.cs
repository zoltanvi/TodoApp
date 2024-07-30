using System.Windows;
using Modules.Common.Services;

namespace Modules.Common.Views.Services;

public class UIScalerBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new UIScalerBindingProxy();
    }

    public IUIScaler UIScaler
    {
        get { return (IUIScaler)GetValue(UIScalerProperty); }
        set { SetValue(UIScalerProperty, value); }
    }

    public static readonly DependencyProperty UIScalerProperty =
        DependencyProperty.Register(nameof(UIScaler), typeof(IUIScaler), typeof(UIScalerBindingProxy), new UIPropertyMetadata(null));
}