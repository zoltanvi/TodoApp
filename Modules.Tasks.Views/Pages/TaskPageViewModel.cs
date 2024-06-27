using System.ComponentModel;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Windows.Input;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel
{
    private bool _isCategoryInEditMode;

    public TaskPageViewModel()
    {
        // TODO: try to make it transient with dispose
        AppSettings.Instance.PageTitleSettings.PropertyChanged += OnPageTitleSettingsChanged;
    }

    private void OnPageTitleSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PageTitleSettings.Visible))
        {
            OnPropertyChanged(nameof(IsCategoryNameTitleVisible));
        }
    }

    public string ActiveCategoryName { get; set; } = "TEST CATEGORY";

    public bool IsCategoryInEditMode
    {
        get => _isCategoryInEditMode;
        set
        {
            if (value == _isCategoryInEditMode) return;
            _isCategoryInEditMode = value;
            OnPropertyChanged(nameof(IsCategoryInEditMode));
            OnPropertyChanged(nameof(IsCategoryInDisplayMode));
            OnPropertyChanged(nameof(IsCategoryNameTitleVisible));
        }
    }

    public bool IsCategoryInDisplayMode => !IsCategoryInEditMode;
    public bool IsCategoryNameTitleVisible => AppSettings.Instance.PageTitleSettings.Visible && IsCategoryInDisplayMode;

    // Commands
    public ICommand EditCategoryCommand { get; }
}
