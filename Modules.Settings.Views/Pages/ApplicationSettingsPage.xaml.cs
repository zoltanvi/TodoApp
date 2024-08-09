using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for ApplicationSettingsPage.xaml
/// </summary>
public partial class ApplicationSettingsPage : INotifyPropertyChanged, IApplicationSettingsPage
{
    public ApplicationSettingsPage(ApplicationSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
