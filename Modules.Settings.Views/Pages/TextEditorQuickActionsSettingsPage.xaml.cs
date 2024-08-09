using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TextEditorQuickActionsSettingsPage.xaml
/// </summary>
public partial class TextEditorQuickActionsSettingsPage : INotifyPropertyChanged, ITextEditorQuickActionsSettingsPage
{
    public TextEditorQuickActionsSettingsPage(TextEditorQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
