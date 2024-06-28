using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Windows.Input;
using MediatR;
using Modules.Categories.Contracts.Cqrs.Queries;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private bool _isCategoryInEditMode;

    public TaskPageViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;

        AppSettings.Instance.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
        var activeCategoryInfo = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

        ActiveCategoryName = activeCategoryInfo.Name;
    }

    private void OnPageTitleSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PageTitleSettings.Visible))
        {
            OnPropertyChanged(nameof(IsCategoryNameTitleVisible));
        }
    }

    public string ActiveCategoryName { get; }

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

    protected override void OnDispose()
    {
        AppSettings.Instance.PageTitleSettings.SettingsChanged -= OnPageTitleSettingsChanged;
    }
}
