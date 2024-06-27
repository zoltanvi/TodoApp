using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Tasks.Views.Pages;

public partial class TaskPage : GenericBasePage<TaskPageViewModel>, ITaskPage
{
    public TaskPage(TaskPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
