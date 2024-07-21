using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Common.Views.DragDrop;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Events;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Mappings;
using Modules.Tasks.Views.Services;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel, IDropIndexModifier
{
    private readonly IMediator _mediator;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly OneEditorOpenService _oneEditorOpenService;

    public event EventHandler? FocusAddNewTaskTextEditorRequested;
    public event Action<int>? ScrollIntoViewRequested;

    public TaskPageViewModel(
        IMediator mediator,
        ITaskItemRepository taskItemRepository,
        OneEditorOpenService oneEditorOpenService)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);

        _mediator = mediator;
        _taskItemRepository = taskItemRepository;
        _oneEditorOpenService = oneEditorOpenService;

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

        // Fixes list orders if necessary, when the task page opens
        var orderedTasks = AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState
            ? OrderTasksByState(tasks)
            : tasks;

        Items = new ObservableCollection<TaskItemViewModel>(orderedTasks.MapToViewModelList(_mediator, oneEditorOpenService));
        Items.CollectionChanged += ItemsOnCollectionChanged;
        RecalculateProgress();

        PinTaskItemRequestedEvent.PinTaskItemRequested += OnPinTaskItemRequested;
        UnpinTaskItemRequestedEvent.UnpinTaskItemRequested += OnUnpinTaskItemRequested;
        FinishTaskItemRequestedEvent.FinishTaskItemRequested += OnFinishTaskItemRequested;
        UnfinishTaskItemRequestedEvent.UnfinishTaskItemRequested += OnUnfinishTaskItemRequested;
        DeleteTaskItemRequestedEvent.DeleteTaskItemRequested += OnDeleteTaskItemRequestedEvent;
        _oneEditorOpenService.ChangedToDisplayMode += FocusAddNewTaskTextEditor;
    }

    public ObservableCollection<TaskItemViewModel> Items { get; }

    public RichTextEditorViewModel AddNewTaskTextEditorViewModel { get; }

    public string ActiveCategoryName { get; private set; }

    public string RenameCategoryContent { get; set; } = "RenameCategoryContent";

    public bool IsCategoryInEditMode { get; set; }

    public bool IsCategoryInDisplayMode => !IsCategoryInEditMode;
    public bool IsCategoryNameTitleVisible => AppSettings.Instance.PageTitleSettings.Visible && !IsCategoryInEditMode;
    public bool IsCategoryNameTitleEditorVisible => AppSettings.Instance.PageTitleSettings.Visible && IsCategoryInEditMode;

    public int TaskCount { get; private set; } = 1000;
    public int FinishedTaskCount { get; private set; } = 555;

    public bool IsBottomPanelOpen { get; set; } = true;

    // Commands
    public ICommand EditCategoryCommand { get; }
    public ICommand FinishCategoryEditCommand { get; }
    public ICommand ToggleBottomPanelCommand { get; }
    public ICommand AddTaskItemCommand { get; }
    public ICommand TextBoxFocusedCommand { get; }

    private void OnTextBoxFocused() => _oneEditorOpenService.EditModeWithoutTask();

    private void AddTaskItem()
    {
        if (!AddNewTaskTextEditorViewModel.IsContentEmpty)
        {
            var activeCategory = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

            var task = new TaskItem
            {
                Content = AddNewTaskTextEditorViewModel.DocumentContent,
                ContentPreview = AddNewTaskTextEditorViewModel.DocumentContentPreview,
                CategoryId = activeCategory.Id,
                // TODO
                ListOrder = Items.Count
            };

            var addedTask = _taskItemRepository.AddTask(task);

            _oneEditorOpenService.LastEditedTaskId = addedTask.Id;

            Items.Add(addedTask.MapToViewModel(_mediator, _oneEditorOpenService));
            RecalculateProgress();

            AddNewTaskTextEditorViewModel.DocumentContent = string.Empty;
        }
    }

    private void OnPinTaskItemRequested(PinTaskItemRequestedEvent request)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == request.TaskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.Pinned = true;
        taskItem.IsDone = false;

        _taskItemRepository.UpdateTaskItem(taskItem.Map());

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Pinned
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnUnpinTaskItemRequested(UnpinTaskItemRequestedEvent request)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == request.TaskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.Pinned = false;
        _taskItemRepository.UpdateTaskItem(taskItem.Map());

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Unpinned
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnFinishTaskItemRequested(FinishTaskItemRequestedEvent request)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == request.TaskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.IsDone = true;
        taskItem.Pinned = false;
        _taskItemRepository.UpdateTaskItem(taskItem.Map());
        RecalculateProgress();

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Done
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnUnfinishTaskItemRequested(UnfinishTaskItemRequestedEvent request)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == request.TaskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.IsDone = false;
        _taskItemRepository.UpdateTaskItem(taskItem.Map());
        RecalculateProgress();

        var query = new TaskInsertPositionQuery
        {
            TaskId = request.TaskId,
            PositionChangeReason = PositionChangeReason.Undone
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnDeleteTaskItemRequestedEvent(DeleteTaskItemRequestedEvent request)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == request.TaskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        Items.Remove(taskItem);
        RecalculateProgress();

        _taskItemRepository.DeleteTask(taskItem.Map());
    }

    private void OnQuickEditRequested()
    {
        var lastAddedTaskItem = Items.FirstOrDefault(x => x.Id == _oneEditorOpenService.LastEditedTaskId);

        if (lastAddedTaskItem != null)
        {
            int index = Items.IndexOf(lastAddedTaskItem);
            lastAddedTaskItem.EditItemCommand.Execute(null);
            ScrollIntoViewRequested?.Invoke(index);
        }
    }

    private void EditCategory()
    {
        IsCategoryInEditMode = true;
        var activeCategory = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;
        RenameCategoryContent = activeCategory.Name;
    }

    private void FinishCategoryEdit()
    {
        if (ActiveCategoryName != RenameCategoryContent)
        {
            var newName = _mediator.Send(new RenameActiveCategoryCommand { Name = RenameCategoryContent }).Result;
            ActiveCategoryName = newName;
        }

        IsCategoryInEditMode = false;
    }

    private void RecalculateProgress()
    {
        TaskCount = Items.Count;
        FinishedTaskCount = Items.Count(x => x.IsDone);
    }

    private void OnPageTitleSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PageTitleSettings.Visible))
        {
            OnPropertyChanged(nameof(IsCategoryNameTitleVisible));
            OnPropertyChanged(nameof(IsCategoryNameTitleEditorVisible));
        }
    }

    private void FocusAddNewTaskTextEditor() => FocusAddNewTaskTextEditorRequested?.Invoke(this, EventArgs.Empty);

    protected override void OnDispose()
    {
        AppSettings.Instance.PageTitleSettings.SettingsChanged -= OnPageTitleSettingsChanged;
        Items.CollectionChanged -= ItemsOnCollectionChanged;

        PinTaskItemRequestedEvent.PinTaskItemRequested -= OnPinTaskItemRequested;
        UnpinTaskItemRequestedEvent.UnpinTaskItemRequested -= OnUnpinTaskItemRequested;
        FinishTaskItemRequestedEvent.FinishTaskItemRequested -= OnFinishTaskItemRequested;
        UnfinishTaskItemRequestedEvent.UnfinishTaskItemRequested -= OnUnfinishTaskItemRequested;
        DeleteTaskItemRequestedEvent.DeleteTaskItemRequested -= OnDeleteTaskItemRequestedEvent;
        _oneEditorOpenService.ChangedToDisplayMode -= FocusAddNewTaskTextEditor;
    }

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
        }

        _taskItemRepository.UpdateTaskListOrders(Items.MapList());
    }

    private List<TaskItem> OrderTasksByState(List<TaskItem> tasks)
    {
        List<TaskItem> orderedTasks = new List<TaskItem>();

        var pinnedItems = tasks.Where(x => x.Pinned);
        var activeItems = tasks.Where(x => !x.IsDone && !x.Pinned);
        var doneItems = tasks.Where(x => x.IsDone);

        orderedTasks.AddRange(pinnedItems);
        orderedTasks.AddRange(activeItems);
        orderedTasks.AddRange(doneItems);

        for (var i = 0; i < orderedTasks.Count; i++)
        {
            orderedTasks[i].ListOrder = i;
        }

        _taskItemRepository.UpdateTaskListOrders(orderedTasks);

        return orderedTasks;
    }

    private void MoveTaskItem(int newIndex, TaskItemViewModel taskItem)
    {
        Items.Remove(taskItem);
        Items.Insert(newIndex, taskItem);
    }

    public int GetModifiedDropIndex(int dropIndex, object droppedObject)
    {
        if (droppedObject is not TaskItemViewModel taskItem)
        {
            throw new ArgumentException($"{nameof(droppedObject)} is not a {nameof(TaskItemViewModel)}");
        }

        var query = new TaskExternalInsertPositionQuery
        {
            TaskId = taskItem.Id,
            RequestedInsertPosition = dropIndex
        };

        return _mediator.Send(query).Result;
    }
}