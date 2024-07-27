using Modules.Common.Navigation;

namespace Modules.Tasks.Views.Pages;

public partial class TaskHistoryPage : ITaskHistoryPage
{
    public TaskHistoryPage(TaskHistoryPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
