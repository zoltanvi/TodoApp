using Modules.Tasks.Views.Controls;

namespace Modules.Tasks.Views.Services;

public class OneEditorOpenService
{
    private TaskItemViewModel? _editorOpenTask;
    private bool _editModeRequested;

    public event Action? ChangedToDisplayMode;

    public int LastEditedTaskId { get; set; }

    public void DisplayMode(TaskItemViewModel taskItem)
    {
        if (_editorOpenTask == taskItem)
        {
            _editorOpenTask = null;

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

        _editModeRequested = false;
    }
}
