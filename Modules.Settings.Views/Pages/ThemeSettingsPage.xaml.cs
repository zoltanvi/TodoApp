using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for ThemeSettingsPage.xaml
/// </summary>
public partial class ThemeSettingsPage : INotifyPropertyChanged, IThemeSettingsPage
{
    public ThemeSettingsPage(ThemeSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
