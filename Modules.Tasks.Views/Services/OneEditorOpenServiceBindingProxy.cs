using System.Windows;

namespace Modules.Tasks.Views.Services;

public class OneEditorOpenServiceBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new OneEditorOpenServiceBindingProxy();
    }

    public OneEditorOpenService Service
    {
        get { return (OneEditorOpenService)GetValue(ServiceProperty); }
        set { SetValue(ServiceProperty, value); }
    }

    public static readonly DependencyProperty ServiceProperty =
        DependencyProperty.Register(nameof(Service), typeof(OneEditorOpenService), typeof(OneEditorOpenServiceBindingProxy), new UIPropertyMetadata(null));
}
