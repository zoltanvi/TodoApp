using Modules.Common;
using Modules.Tasks.TextEditor.Controls;
using System.Windows;

namespace Modules.Tasks.TextEditor.Services;

public class ToolbarService
{
    private static ToolbarService? _instance;
    private static SingletonToolbar? _cachedToolbar;
    private static ToolbarService Instance => _instance ??= new ToolbarService();

    public static SingletonToolbar Toolbar => Instance.GetToolbar();

    private SingletonToolbar GetToolbar()
    {
        if (_cachedToolbar == null)
        {
            _cachedToolbar = Application.Current.TryFindResource(Constants.ResourceNames.TextEditorToolbar) as SingletonToolbar;
            ArgumentNullException.ThrowIfNull(_cachedToolbar);
        }

        return _cachedToolbar;
    }
}
