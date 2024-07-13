using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Tasks.Views.Pages;

public partial class TaskPage : GenericBasePage<TaskPageViewModel>, ITaskPage, IDisposable
{
    public TaskPage(TaskPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        
        viewModel.FocusAddNewTaskTextEditorRequested += OnFocusAddNewTaskTextEditor;
        viewModel.ScrollIntoViewRequested += OnScrollIntoViewRequested;
    }

    private void OnScrollIntoViewRequested(int index)
    {
        var itemToScrollTo = TaskListListView.Items.GetItemAt(index);
        if (itemToScrollTo != null)
        {
            TaskListListView.ScrollIntoView(itemToScrollTo);
        }
    }

    private void OnFocusAddNewTaskTextEditor(object? sender, EventArgs e)
    {
        BottomTextEditor.SetFocus();
    }

    public void Dispose()
    {
        ViewModel.FocusAddNewTaskTextEditorRequested -= OnFocusAddNewTaskTextEditor;
    }
}
