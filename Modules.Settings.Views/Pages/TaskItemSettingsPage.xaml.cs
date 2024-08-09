using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskItemSettingsPage.xaml
/// </summary>
public partial class TaskItemSettingsPage : INotifyPropertyChanged, ITaskItemSettingsPage
{
    public TaskItemSettingsPage(TaskItemSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
