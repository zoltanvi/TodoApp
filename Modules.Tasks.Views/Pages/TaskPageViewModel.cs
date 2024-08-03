using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common.DataBinding;
using Modules.Common.Events;
using Modules.Common.Helpers;
using Modules.Common.ViewModel;
using Modules.Common.Views.DragDrop;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Controls;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Mappings;
using Modules.Tasks.Views.Services;
using Prism.Events;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Modules.Tasks.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class TaskPageViewModel : BaseViewModel, IDropIndexModifier
{
    private readonly IMediator _mediator;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly OneEditorOpenService _oneEditorOpenService;
    private readonly IEventAggregator _eventAggregator;
    private bool _sortingInProgress;
    private string _searchText;
    private List<string> _searchTerms = new();
    private bool _isSearchBoxOpen;

    public event EventHandler? FocusAddNewTaskTextEditorRequested;
    public event Action<int>? ScrollIntoViewRequested;

    public TaskPageViewModel(
        IMediator mediator,
        ITaskItemRepository taskItemRepository,
        OneEditorOpenService oneEditorOpenService,
        IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        ArgumentNullException.ThrowIfNull(oneEditorOpenService);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        _mediator = mediator;
        _taskItemRepository = taskItemRepository;
        _oneEditorOpenService = oneEditorOpenService;
        _eventAggregator = eventAggregator;

        AppSettings.Instance.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
        var activeCategoryInfo = _mediator.Send(new GetActiveCategoryInfoQuery()).Result;

        ActiveCategoryName = activeCategoryInfo.Name;

        EditCategoryCommand = new RelayCommand(EditCategory);
        FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);
        ToggleBottomPanelCommand = new RelayCommand(() => IsBottomPanelOpen ^= true);
        AddTaskItemCommand = new RelayCommand(AddTaskItem);
        TextBoxFocusedCommand = new RelayCommand(OnTextBoxFocused);
        CloseSearchBoxCommand = new RelayCommand(() =>
        {
            IsSearchBoxOpen = false;
            SearchText = string.Empty;
        });

        AddNewTaskTextEditorViewModel = new RichTextEditorViewModel(false, false, true, true);
        AddNewTaskTextEditorViewModel.WatermarkText = "Add new task";
        AddNewTaskTextEditorViewModel.EnterAction = AddTaskItem;
        AddNewTaskTextEditorViewModel.OnQuickEditRequestedAction = OnQuickEditRequested;

        var tasks = _taskItemRepository.GetActiveTasksFromCategory(activeCategoryInfo.Id, includeNavigation: true);

        // Fixes list orders if necessary, when the task page opens
        var orderedTasks = AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState
            ? OrderTasksByState(tasks)
            : tasks;

        Items = new ObservableCollection<TaskItemViewModel>(
            orderedTasks.MapToViewModelList(_mediator, oneEditorOpenService, _eventAggregator));

        Items.CollectionChanged += ItemsOnCollectionChanged;
        SetFirstItem();
        RecalculateProgress();

        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterTaskItems;

        _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Subscribe(OnDeleteTaskItemRequestedEvent);
        _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Subscribe(OnPinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Subscribe(OnUnpinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCheckedEvent>().Subscribe(OnFinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Subscribe(OnUnfinishTaskItemRequested);
        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Subscribe(OnTagsChangedOnTaskItem);
        _eventAggregator.GetEvent<TaskSortingRequestedEvent>().Subscribe(OnSortingRequested);
        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Subscribe(OnVersionRestored);
        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Subscribe(OnCtrlFPressed);

        _oneEditorOpenService.ChangedToDisplayMode += FocusAddNewTaskTextEditor;
    }

    public bool IsSearchBoxOpen
    {
        get => _isSearchBoxOpen;
        set
        {
            if (value == false)
            {
                SearchText = string.Empty;
            }

            _isSearchBoxOpen = value;
        }
    }

    private bool FilterTaskItems(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchText)) return true;

        if (obj is TaskItemViewModel taskItem)
        {
            HashSet<string> searchTerms = SearchText.GetSearchTerms();

            return searchTerms.All(term => taskItem.ContentPreview.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    public ObservableCollection<TaskItemViewModel> Items { get; }

    public ICollectionView ItemsView { get; set; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!IsSearchBoxOpen) return;

            _searchText = value;
            
            var newSearchTerms = _searchText.GetSearchTermsList();
            if (!newSearchTerms.SequenceEqual(_searchTerms))
            {
                ItemsView.Refresh();
            }

            _searchTerms = newSearchTerms;
        }
    }

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
    public ICommand CloseSearchBoxCommand { get; }

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

            Items.Add(addedTask.MapToViewModel(_mediator, _oneEditorOpenService, _eventAggregator));
            ScrollIntoViewRequested?.Invoke(Items.Count - 1);
            RecalculateProgress();

            AddNewTaskTextEditorViewModel.DocumentContent = string.Empty;
        }
    }

    private void OnPinTaskItemRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.Pinned = true;
        taskItem.IsDone = false;

        var updatedDbTask = _taskItemRepository.UpdateTaskItem(taskItem.Map());
        taskItem.ModificationDate = updatedDbTask.ModificationDate;

        var query = new TaskInsertPositionQuery
        {
            TaskId = taskId,
            PositionChangeReason = PositionChangeReason.Pinned
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnUnpinTaskItemRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.Pinned = false;
        var updatedDbTask = _taskItemRepository.UpdateTaskItem(taskItem.Map());
        taskItem.ModificationDate = updatedDbTask.ModificationDate;

        var query = new TaskInsertPositionQuery
        {
            TaskId = taskId,
            PositionChangeReason = PositionChangeReason.Unpinned
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnFinishTaskItemRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.IsDone = true;
        taskItem.Pinned = false;
        var updatedDbTask = _taskItemRepository.UpdateTaskItem(taskItem.Map());
        taskItem.ModificationDate = updatedDbTask.ModificationDate;
        RecalculateProgress();

        var query = new TaskInsertPositionQuery
        {
            TaskId = taskId,
            PositionChangeReason = PositionChangeReason.Done
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnUnfinishTaskItemRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        taskItem.IsDone = false;
        var updatedDbTask = _taskItemRepository.UpdateTaskItem(taskItem.Map());
        taskItem.ModificationDate = updatedDbTask.ModificationDate;
        RecalculateProgress();

        var query = new TaskInsertPositionQuery
        {
            TaskId = taskId,
            PositionChangeReason = PositionChangeReason.Undone
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnDeleteTaskItemRequestedEvent(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
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
            lastAddedTaskItem.Cmd.EditItemCommand.Execute(null);
            ScrollIntoViewRequested?.Invoke(index);
        }
    }

    private void OnTagsChangedOnTaskItem(int taskId)
    {
        var task = Items.FirstOrDefault(x => x.Id == taskId);
        if (task != null)
        {
            var index = Items.IndexOf(task);
            var updatedTask = _taskItemRepository.GetTaskById(taskId, includeNavigation: true);
            ArgumentNullException.ThrowIfNull(updatedTask);

            Items[index].Tags = updatedTask.Tags.MapTagItems();
        }
    }

    private void OnSortingRequested(TaskSortingRequestedPayload request)
    {
        IEnumerable<TaskItemViewModel> sortedItems = Items;

        switch (request.SortBy)
        {
            case TaskSortingRequestedPayload.SortByProperty.State:
            {
                if (AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState) break;

                var pinnedItems = Items.Where(x => x.Pinned && !x.IsDone);
                var unfinishedItems = Items.Where(x => !x.Pinned && !x.IsDone);
                var finishedItems = Items.Where(x => !x.Pinned && x.IsDone);

                sortedItems = pinnedItems.Concat(unfinishedItems).Concat(finishedItems);
                break;
            }
            case TaskSortingRequestedPayload.SortByProperty.CreationDate:
            {
                sortedItems = request.Ascending
                    ? Items.OrderBy(x => x.CreationDate)
                    : Items.OrderByDescending(x => x.CreationDate);
                break;
            }
            case TaskSortingRequestedPayload.SortByProperty.ModificationDate:
            {
                sortedItems = request.Ascending
                    ? Items.OrderBy(x => x.ModificationDate)
                    : Items.OrderByDescending(x => x.ModificationDate);
                break;
            }
            case TaskSortingRequestedPayload.SortByProperty.Content:
            {
                sortedItems = request.Ascending
                    ? Items.OrderBy(x => x.ContentPreview)
                    : Items.OrderByDescending(x => x.ContentPreview);
                break;
            }
            default:
            throw new NotImplementedException($"Sorting is not implemented on {request.SortBy}.");
        }

        // If forced task order by state is turned on, re-sort the sorted items by state
        if (request.SortBy != TaskSortingRequestedPayload.SortByProperty.State)
        {
            if (AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState)
            {
                var pinnedItems = sortedItems.Where(x => x.Pinned && !x.IsDone);
                var unfinishedItems = sortedItems.Where(x => !x.Pinned && !x.IsDone);
                var finishedItems = sortedItems.Where(x => !x.Pinned && x.IsDone);

                sortedItems = pinnedItems.Concat(unfinishedItems).Concat(finishedItems);
            }
            else
            {
                var pinnedItems = sortedItems.Where(x => x.Pinned);
                var notPinnedItems = sortedItems.Where(x => !x.Pinned);

                sortedItems = pinnedItems.Concat(notPinnedItems);
            }
        }

        sortedItems = sortedItems.ToList();

        _sortingInProgress = true;
        
        Items.Clear();
        foreach (var taskItem in sortedItems)
        {
            Items.Add(taskItem);
        }
        
        _sortingInProgress = false;
        ItemsOnCollectionChanged(null, null);
    }

    private void OnVersionRestored(int taskId)
    {
        var dbTask = _taskItemRepository.GetTaskById(taskId, includeNavigation: true);
        ArgumentNullException.ThrowIfNull(dbTask);
        
        var updatedTask = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(updatedTask);

        updatedTask.Content = dbTask.Content;
        updatedTask.ContentPreview = dbTask.ContentPreview;
        updatedTask.ModificationDate = dbTask.ModificationDate;
        updatedTask.Versions = dbTask.Versions.MapToViewModelList(_mediator);
    }

    private void OnCtrlFPressed()
    {
        IsSearchBoxOpen = true;
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

        _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Unsubscribe(OnDeleteTaskItemRequestedEvent);
        _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Unsubscribe(OnPinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Unsubscribe(OnUnpinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCheckedEvent>().Unsubscribe(OnFinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Unsubscribe(OnUnfinishTaskItemRequested);
        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Unsubscribe(OnTagsChangedOnTaskItem);
        _eventAggregator.GetEvent<TaskSortingRequestedEvent>().Unsubscribe(OnSortingRequested);
        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Unsubscribe(OnVersionRestored);
        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Unsubscribe(OnCtrlFPressed);

        _oneEditorOpenService.ChangedToDisplayMode -= FocusAddNewTaskTextEditor;
    }

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
    {
        if (_sortingInProgress) return;

        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
            Items[i].IsFirstItem = i == 0;
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

        var query = new TaskDragDropInsertPositionQuery
        {
            TaskId = taskItem.Id,
            RequestedInsertPosition = dropIndex
        };

        return _mediator.Send(query).Result;
    }

    private void SetFirstItem()
    {
        var firstItem = Items.FirstOrDefault();
        if (firstItem != null)
        {
            firstItem.IsFirstItem = true;
        }
    }
}