using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Mappings;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskPageViewModel(IMediator mediator, ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(taskItemRepository);

        _mediator = mediator;
        _taskItemRepository = taskItemRepository;

        AppSettings.Instance.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
        var activeCategoryInfo = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

        ActiveCategoryName = activeCategoryInfo.Name;

        EditCategoryCommand = new RelayCommand(EditCategory);
        FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);
        ToggleBottomPanelCommand = new RelayCommand(() => IsBottomPanelOpen ^= true);
        AddTaskItemCommand = new RelayCommand(AddTaskItem);
        TextBoxFocusedCommand = new RelayCommand(OnTextBoxFocused);

        AddNewTaskTextEditorViewModel = new RichTextEditorViewModel(false, false, true, true);
        AddNewTaskTextEditorViewModel.WatermarkText = "Add new task";
        AddNewTaskTextEditorViewModel.EnterAction = AddTaskItem;
        AddNewTaskTextEditorViewModel.OnQuickEditRequestedAction = OnQuickEditRequested;

        var tasks = _taskItemRepository.GetActiveTasksFromCategory(activeCategoryInfo.Id);
        Items = new ObservableCollection<TaskItemViewModel>(tasks.MapToViewModelList());
    }

    public ObservableCollection<TaskItemViewModel> Items { get; }

    public RichTextEditorViewModel AddNewTaskTextEditorViewModel { get; }

    public string ActiveCategoryName { get; private set; }

    public string RenameCategoryContent { get; set; } = "RenameCategoryContent";

    public bool IsCategoryInEditMode { get; set; }

    public bool IsCategoryInDisplayMode => !IsCategoryInEditMode;
    public bool IsCategoryNameTitleVisible => AppSettings.Instance.PageTitleSettings.Visible && !IsCategoryInEditMode;
    public bool IsCategoryNameTitleEditorVisible => AppSettings.Instance.PageTitleSettings.Visible && IsCategoryInEditMode;

    public int TaskCount { get; } = 10;
    public int FinishedTaskCount { get; } = 5;

    public bool IsBottomPanelOpen { get; set; } = true;

    // Commands
    public ICommand EditCategoryCommand { get; }
    public ICommand FinishCategoryEditCommand { get; }
    public ICommand ToggleBottomPanelCommand { get; }
    public ICommand AddTaskItemCommand { get; }
    public ICommand TextBoxFocusedCommand { get; }


    private void OnTextBoxFocused()
    {
        //IoC.OneEditorOpenService.EditMode(null);
    }

    private void AddTaskItem()
    {
        if (!AddNewTaskTextEditorViewModel.IsContentEmpty)
        {
            var activeCategory = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

            var task = new TaskItem
            {
                Content = AddNewTaskTextEditorViewModel.DocumentContent,
                ContentPreview = "TODO",
                CategoryId = activeCategory.Id,
                // TODO
                ListOrder = Items.Count
            };

            var addedTask = _taskItemRepository.AddTask(task);

            Items.Add(addedTask.MapToViewModel());
        }
    }

    private void OnQuickEditRequested()
    {
        // TODO
    }

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
