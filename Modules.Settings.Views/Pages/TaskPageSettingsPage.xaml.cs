using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskPageSettingsPage.xaml
/// </summary>
public partial class TaskPageSettingsPage : INotifyPropertyChanged, ITaskPageSettingsPage
{
    public TaskPageSettingsPage(TaskPageSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
