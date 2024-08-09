using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for DateTimeSettingsPage.xaml
/// </summary>
public partial class DateTimeSettingsPage : INotifyPropertyChanged, IDateTimeSettingsPage
{
    public DateTimeSettingsPage(DateTimeSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
