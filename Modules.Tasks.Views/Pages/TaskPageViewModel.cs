using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private bool _isCategoryInEditMode;
    private string _renameCategoryContent = "RenameCategoryContent";
    private string _activeCategoryName;

    public TaskPageViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;

        AppSettings.Instance.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
        var activeCategoryInfo = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

        ActiveCategoryName = activeCategoryInfo.Name;

        EditCategoryCommand = new RelayCommand(EditCategory);
        FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);
    }

    public string ActiveCategoryName
    {
        get => _activeCategoryName;
        private set
        {
            if (value == _activeCategoryName) return;
            _activeCategoryName = value;
            OnPropertyChanged(nameof(ActiveCategoryName));
        }
    }

    public string RenameCategoryContent
    {
        get => _renameCategoryContent;
        set
        {
            if (value == _renameCategoryContent) return;
            _renameCategoryContent = value;
            OnPropertyChanged(nameof(RenameCategoryContent));
        }
    }

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
            OnPropertyChanged(nameof(IsCategoryNameTitleEditorVisible));
        }
    }

    public bool IsCategoryInDisplayMode => !IsCategoryInEditMode;
    public bool IsCategoryNameTitleVisible => AppSettings.Instance.PageTitleSettings.Visible && !IsCategoryInEditMode;
    public bool IsCategoryNameTitleEditorVisible => AppSettings.Instance.PageTitleSettings.Visible && IsCategoryInEditMode;

    // Commands
    public ICommand EditCategoryCommand { get; }
    public ICommand FinishCategoryEditCommand { get; }

    private void EditCategory()
    {
        IsCategoryInEditMode = true;
        var activeCategory = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;
        RenameCategoryContent = activeCategory.Name;
    }

    private void FinishCategoryEdit()
    {
        var newName = _mediator.Send(new RenameActiveCategoryCommand { Name = RenameCategoryContent }).Result;
        ActiveCategoryName = newName;
        IsCategoryInEditMode = false;
    }

    private void OnPageTitleSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PageTitleSettings.Visible))
        {
            OnPropertyChanged(nameof(IsCategoryNameTitleVisible));
            OnPropertyChanged(nameof(IsCategoryNameTitleEditorVisible));
        }
    }

    protected override void OnDispose()
    {
        AppSettings.Instance.PageTitleSettings.SettingsChanged -= OnPageTitleSettingsChanged;
    }
}
