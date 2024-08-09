using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : GenericBasePage<SettingsPageViewModel>, INotifyPropertyChanged, ISettingsPage
{
    public SettingsPage(
        SettingsPageViewModel viewModel,
        ISettingsPageNavigationService settingsPageNavigationService) 
        : base(viewModel)
    {
        ArgumentNullException.ThrowIfNull(settingsPageNavigationService);

        InitializeComponent();

        settingsPageNavigationService.Initialize(SettingsPageFrame);
        settingsPageNavigationService.NavigateTo<IApplicationSettingsPage>();

    }

    public event PropertyChangedEventHandler PropertyChanged;
}