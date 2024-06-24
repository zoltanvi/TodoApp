using Modules.Common.DataBinding;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using Modules.Common.Views.Pages;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Views.Controls;
using PropertyChanged;
using System.Windows.Input;
using MediatR;
using Modules.PopupMessage.Contracts.Cqrs.Commands;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private int _activeCategoryId;
    private readonly IMediator _mediator;

    public SettingsPageViewModel(IServiceProvider serviceProvider, IMediator mediator)
    {
        _mediator = mediator;
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;

        GoBackCommand = new RelayCommand(() =>
        {
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.UpdateMainPage);
        });

        OpenPageCommand = new RelayParameterizedCommand<SettingsPageItemViewModel>(OpenSettingsPage);

        Items =
        [
            new() { Id = 1, Name = "APPLICATION", PageType = typeof(ApplicationSettingsPage)},
            new() { Id = 2, Name = "THEME" , PageType = typeof(ThemeSettingsPage)},
            new() { Id = 3, Name = "PAGE TITLE", PageType = typeof(PageTitleSettingsPage) },
            new() { Id = 4, Name = "TASK PAGE" , PageType = typeof(TaskPageSettingsPage)},
            new() { Id = 5, Name = "TASKS" , PageType = typeof(TaskItemSettingsPage)},
            new() { Id = 6, Name = "QUICK ACTIONS", PageType = typeof(TaskQuickActionsSettingsPage) },
            new() { Id = 7, Name = "EDITOR", PageType = typeof(TextEditorQuickActionsSettingsPage) },
            new() { Id = 8, Name = "NOTES", PageType = typeof(NotePageSettingsPage) },
            new() { Id = 9, Name = "DATE TIME", PageType = typeof(DateTimeSettingsPage) },
            new() { Id = 10, Name = "SHORTCUTS" , PageType = typeof(ShortcutsPage)}
        ];

        // Open ApplicationSettingsPage by default
        ActiveCategoryId = 1;
    }

    private void OpenSettingsPage(SettingsPageItemViewModel item)
    {
        _mediator.Send(new ShowMessageErrorCommand
        {
            Message = $"Opened: {item.Name}."
        });

        ActiveCategoryId = item.Id;
    }

    public ICommand OpenPageCommand { get; }
    public ICommand GoBackCommand { get; }

    public BasePage? SettingsPageFrameContent { get; set; }

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

            OnPropertyChanged(nameof(ActiveCategoryId));
            OnPropertyChanged(nameof(SettingsPageFrameContent));
        }
    }

    private BasePage? GetSettingsPage(int id)
    {
        var itemViewModel = Items.FirstOrDefault(x => x.Id == id);

        ArgumentNullException.ThrowIfNull(itemViewModel);

        return (BasePage)_serviceProvider.GetService(itemViewModel.PageType);
        
    }
}
