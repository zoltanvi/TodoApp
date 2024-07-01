using Modules.Common.DataBinding;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Common.Views.Pages;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Views.Controls;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private int _activeCategoryId;

    public SettingsPageViewModel(
        IServiceProvider serviceProvider, 
        IMainPageNavigationService mainPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);

        _serviceProvider = serviceProvider;
        _mainPageNavigationService = mainPageNavigationService;

        GoBackCommand = new RelayCommand(() => _mainPageNavigationService.GoBackToPreviousPage());

        OpenPageCommand = new RelayParameterizedCommand<SettingsPageItemViewModel>(OpenSettingsPage);

        Items = new List<SettingsPageItemViewModel>
        {
            new() { Id = 1, Name = "APPLICATION", PageType = typeof(ApplicationSettingsPage) },
            new() { Id = 2, Name = "THEME", PageType = typeof(ThemeSettingsPage) },
            new() { Id = 3, Name = "PAGE TITLE", PageType = typeof(PageTitleSettingsPage) },
            new() { Id = 4, Name = "TASK PAGE", PageType = typeof(TaskPageSettingsPage) },
            new() { Id = 5, Name = "TASKS", PageType = typeof(TaskItemSettingsPage) },
            new() { Id = 6, Name = "QUICK ACTIONS", PageType = typeof(TaskQuickActionsSettingsPage) },
            new() { Id = 7, Name = "EDITOR", PageType = typeof(TextEditorQuickActionsSettingsPage) },
            new() { Id = 8, Name = "NOTES", PageType = typeof(NotePageSettingsPage) },
            new() { Id = 9, Name = "DATE TIME", PageType = typeof(DateTimeSettingsPage) },
            new() { Id = 10, Name = "SHORTCUTS", PageType = typeof(ShortcutsPage) }
        };

        // Open ApplicationSettingsPage by default
        ActiveCategoryId = 1;
    }

    private void OpenSettingsPage(SettingsPageItemViewModel item)
    {
        ActiveCategoryId = item.Id;
    }

    public ICommand OpenPageCommand { get; }
    public ICommand GoBackCommand { get; }

    public BasePage? SettingsPageFrameContent { get; private set; }

    public List<SettingsPageItemViewModel> Items { get; }

    public int ActiveCategoryId
    {
        get => _activeCategoryId;
        private set
        {
            if (value == _activeCategoryId) return;

            _activeCategoryId = value;
            AppSettings.Instance.SessionSettings.ActiveSettingsCategoryId = value;

            SettingsPageFrameContent = GetSettingsPage(_activeCategoryId);
        }
    }

    private BasePage? GetSettingsPage(int id)
    {
        SettingsPageItemViewModel? itemViewModel = Items.FirstOrDefault(x => x.Id == id);

        ArgumentNullException.ThrowIfNull(itemViewModel);

        return (BasePage)_serviceProvider.GetService(itemViewModel.PageType);

    }
}
