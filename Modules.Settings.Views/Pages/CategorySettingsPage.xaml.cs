using Modules.Common.Navigation;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

public partial class CategorySettingsPage : INotifyPropertyChanged, ICategorySettingsPage
{
    public CategorySettingsPage(CategorySettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
