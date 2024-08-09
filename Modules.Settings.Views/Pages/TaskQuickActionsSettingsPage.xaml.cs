using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskQuickActionsSettingsPage.xaml
/// </summary>
public partial class TaskQuickActionsSettingsPage : INotifyPropertyChanged, ITaskQuickActionsSettingsPage
{
    public TaskQuickActionsSettingsPage(TaskQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
