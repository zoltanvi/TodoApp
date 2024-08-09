using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Views.Controls;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel : BaseViewModel, ICloseRequester
{
    private readonly ISettingsPageNavigationService _navigation;
    private int _activeCategoryId;

    public SettingsPageViewModel(ISettingsPageNavigationService navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);

        _navigation = navigation;

        OpenPageCommand = new RelayParameterizedCommand<SettingsPageItemViewModel>(OpenSettingsPage);

        Items = new List<SettingsPageItemViewModel>
        {
            new() { Id = 1, Name = "APPLICATION", NavigateAction = () => _navigation.NavigateTo<IApplicationSettingsPage>() },
            new() { Id = 2, Name = "THEME", NavigateAction = () => _navigation.NavigateTo<IThemeSettingsPage>() },
            new() { Id = 3, Name = "PAGE TITLE", NavigateAction = () => _navigation.NavigateTo<IPageTitleSettingsPage>() },
            new() { Id = 4, Name = "TASK PAGE", NavigateAction = () => _navigation.NavigateTo<ITaskPageSettingsPage>() },
            new() { Id = 5, Name = "TASKS", NavigateAction = () => _navigation.NavigateTo<ITaskItemSettingsPage>() },
            new() { Id = 6, Name = "TAGS", NavigateAction = () => _navigation.NavigateTo<ITagSettingsPage>() },
            new() { Id = 7, Name = "QUICK ACTIONS", NavigateAction = () => _navigation.NavigateTo<ITaskQuickActionsSettingsPage>() },
            new() { Id = 8, Name = "EDITOR", NavigateAction = () => _navigation.NavigateTo<ITextEditorQuickActionsSettingsPage>() },
            new() { Id = 10, Name = "DATE TIME", NavigateAction = () => _navigation.NavigateTo<IDateTimeSettingsPage>() },
            new() { Id = 11, Name = "SHORTCUTS", NavigateAction = () => _navigation.NavigateTo<IShortcutsPage>() }
        };

        _activeCategoryId = 1;
    }

    private void OpenSettingsPage(SettingsPageItemViewModel item)
    {
        ActiveCategoryId = item.Id;
    }

    public ICommand OpenPageCommand { get; }
    public ICommand ClosePageCommand { get; set; }
    public List<SettingsPageItemViewModel> Items { get; }

    public int ActiveCategoryId
    {
        get => _activeCategoryId;
        private set
        {
            if (value == _activeCategoryId) return;

            _activeCategoryId = value;
            AppSettings.Instance.SessionSettings.ActiveSettingsCategoryId = value;
            
            NavigateToCategory(_activeCategoryId);
        }
    }

    private void NavigateToCategory(int id)
    {
        SettingsPageItemViewModel? itemViewModel = Items.FirstOrDefault(x => x.Id == id);
        ArgumentNullException.ThrowIfNull(itemViewModel);

        itemViewModel.NavigateAction.Invoke();
    }
}
