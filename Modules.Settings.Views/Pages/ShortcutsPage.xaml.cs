using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class ShortcutsPage : INotifyPropertyChanged, IShortcutsPage
{
    public ShortcutsPage(ShortcutsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}