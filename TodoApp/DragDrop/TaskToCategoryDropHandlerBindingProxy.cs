using System.Windows;

namespace TodoApp.DragDrop;

public class TaskToCategoryDropHandlerBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new TaskToCategoryDropHandlerBindingProxy();
    }

    public TaskToCategoryDropHandler Handler
    {
        get { return (TaskToCategoryDropHandler)GetValue(HandlerProperty); }
        set { SetValue(HandlerProperty, value); }
    }

    public static readonly DependencyProperty HandlerProperty =
        DependencyProperty.Register(nameof(Handler), typeof(TaskToCategoryDropHandler), typeof(TaskToCategoryDropHandlerBindingProxy), new UIPropertyMetadata(null));
}
