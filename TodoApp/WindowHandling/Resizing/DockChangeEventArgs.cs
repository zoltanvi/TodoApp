namespace TodoApp.WindowHandling.Resizing;

public class DockChangeEventArgs : EventArgs
{
    public DockChangeEventArgs(bool isDocked)
    {
        IsDocked = isDocked;
    }

    public bool IsDocked { get; }
}