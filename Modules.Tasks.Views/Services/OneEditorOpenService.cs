using Modules.Common.ViewModel;
using Modules.Tasks.Views.Controls;
using PropertyChanged;

namespace Modules.Tasks.Views.Services;

[AddINotifyPropertyChangedInterface]
public class OneEditorOpenService : BaseViewModel
{
    private TaskItemViewModel? _editorOpenTask;
    private bool _editModeRequested;
    private OneEditorOpenService() { }
    
    public event Action? ChangedToDisplayMode;
    public static OneEditorOpenService Instance { get; } = new();
    public int LastEditedTaskId { get; set; }
    public bool NoTaskIsUnderEdit => _editorOpenTask == null;

    public void DisplayMode(TaskItemViewModel taskItem)
    {
        if (_editorOpenTask == taskItem)
        {
            _editorOpenTask = null;
            OnPropertyChanged(nameof(NoTaskIsUnderEdit));

            if (!_editModeRequested)
            {
                ChangedToDisplayMode?.Invoke();
            }
        }
    }

    public void EditMode(TaskItemViewModel taskItem)
    {
        _editModeRequested = true;

        // Save changes and close editor for old task
        if (_editorOpenTask != null && 
            _editorOpenTask != taskItem)
        {
            _editorOpenTask.ExitEditItem();
        }

        _editorOpenTask = taskItem;
        OnPropertyChanged(nameof(NoTaskIsUnderEdit));

        if (_editorOpenTask != null)
        {
            LastEditedTaskId = taskItem.Id;
        }

        _editModeRequested = false;
    }

    public void EditModeWithoutTask()
    {
        _editModeRequested = true;

        // Save changes and close editor for old task
        _editorOpenTask?.ExitEditItem();
        _editorOpenTask = null;
        OnPropertyChanged(nameof(NoTaskIsUnderEdit));

        _editModeRequested = false;
    }
}
