using Modules.Tasks.TextEditor.Controls;
using System.Windows.Input;

namespace Modules.Tasks.Views.Controls;

public class TaskItemViewModel
{
    private string _contentRollback = string.Empty;
    private bool _isDone;

    public string MarkerColor { get; set; }
    public string BorderColor { get; set; }
    public string BackgroundColor { get; set; }

    public RichTextEditorViewModel TextEditorViewModel { get; }

    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;
            TextEditorViewModel.TextOpacity = IsDone ? 0.25 : 1.0;
        }
    }

    // IsReminderOn AND TextEditorViewModel.IsDisplayMode
    public bool IsAnyReminderOn { get; set; }

    // Pinned AND TextEditorViewModel.IsDisplayMode
    public bool Pinned { get; set; }

    // AppSettings.TaskQuickActionSettings.AnyEnabled AND TextEditorViewModel.IsDisplayMode
    public bool IsHiddenButtonPanelVisible { get; set; }



    // Commands
    public ICommand IsDoneModifiedCommand { get; }
    public ICommand EditItemCommand { get; }
    public ICommand ToggleIsDoneCommand { get; }
    public ICommand OpenReminderCommand { get; }
    public ICommand PinItemCommand { get; }
    public ICommand UnpinItemCommand { get; }
    public ICommand DeleteItemCommand { get; }

}
