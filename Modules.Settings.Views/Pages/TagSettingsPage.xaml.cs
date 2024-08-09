using Modules.Common.Navigation;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TagSettingsPage.xaml
/// </summary>
public partial class TagSettingsPage : ITagSettingsPage
{
    public TagSettingsPage(TagSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
