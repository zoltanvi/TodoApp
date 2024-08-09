using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for PageTitleSettingsPage.xaml
/// </summary>
public partial class PageTitleSettingsPage : INotifyPropertyChanged, IPageTitleSettingsPage
{
    public PageTitleSettingsPage(PageTitleSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
