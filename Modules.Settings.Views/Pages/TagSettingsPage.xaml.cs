using Modules.Common.Views.Pages;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TagSettingsPage.xaml
/// </summary>
public partial class TagSettingsPage : GenericBasePage<TagSettingsPageViewModel>
{
    public TagSettingsPage(TagSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
