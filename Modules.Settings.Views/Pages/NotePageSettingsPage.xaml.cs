using Modules.Common.Views.Pages;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for NotePageSettingsPage.xaml
/// </summary>
public partial class NotePageSettingsPage : GenericBasePage<NotePageSettingsPageViewModel>
{
    public NotePageSettingsPage(NotePageSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
