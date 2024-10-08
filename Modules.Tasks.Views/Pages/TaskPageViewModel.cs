﻿using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.Events;
using Modules.Common.Extensions;
using Modules.Common.Helpers;
using Modules.Common.ViewModel;
using Modules.Common.Views.Controls;
using Modules.Common.Views.DragDrop;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Queries;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Services.Events;
using Modules.Tasks.Services.Extensions;
using Modules.Tasks.TextEditor.Controls;
using Modules.Tasks.Views.Controls.TaskItemView;
using Modules.Tasks.Views.Events;
using Modules.Tasks.Views.Extensions;
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

    // For improved performance, the code which updates Items in a loop
    // should be surrounded with an '_ignoreCollectionChange = true scope' and update Items after that ONCE.
    private bool _ignoreCollectionChange;

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

        var activeCategoryInfo = _mediator.Send(new GetSelectedCategoryQuery()).Result;

        ActiveCategoryName = activeCategoryInfo.Name;

        NewContentViewModel = new DynamicTextBoxViewModel(
            focusOnEditMode: false,
            enterActionOnLostFocus: false,
            toolbarCloseOnLostFocus: true,
            acceptsTab: true,
            isPlainTextMode: true,
            enterAction: AddTaskItem);

        NewContentViewModel.WatermarkText = "Add new task";
        NewContentViewModel.OnQuickEditRequestedAction = OnQuickEditRequested;

        var tasks = _taskItemRepository.GetActiveTasksFromCategory(activeCategoryInfo.Id, includeNavigation: true);

        // Fixes list orders if necessary, when the task page opens
        var orderedTasks = AppSettings.Instance.TaskPageSettings.ForceTaskOrderByState
            ? OrderTasksByState(tasks)
            : tasks;

        Items = new ObservableCollection<TaskItemViewModel>(
            orderedTasks.MapToViewModelList(_mediator, oneEditorOpenService, _eventAggregator));

        EditCategoryCommand = new RelayCommand(EditCategory);
        FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);
        ToggleBottomPanelCommand = new RelayCommand(() => IsBottomPanelOpen ^= true);
        AddTaskItemCommand = new RelayCommand(AddTaskItem);
        TextBoxFocusedCommand = new RelayCommand(OnTextBoxFocused);
        SwitchFormatMode = new RelayCommand(() => NewContentViewModel.IsPlainTextMode ^= true);

        SetFirstItem();
        RecalculateProgress();

        SearchBoxViewModel = new SearchBoxViewModel();
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterTaskItem;

        SubscribeToEvents();
    }

    public SearchBoxViewModel SearchBoxViewModel { get; set; }

    public ObservableCollection<TaskItemViewModel> Items { get; }

    public ICollectionView ItemsView { get; set; }

    public DynamicTextBoxViewModel NewContentViewModel { get; }

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
    public ICommand SwitchFormatMode { get; }

    private void SubscribeToEvents()
    {
        AppSettings.Instance.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
        Items.CollectionChanged += ItemsOnCollectionChanged;

        _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Subscribe(OnDeleteTaskItemRequestedEvent);
        _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Subscribe(OnPinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Subscribe(OnUnpinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCheckedEvent>().Subscribe(OnFinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Subscribe(OnUnfinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCategoryChangedEvent>().Subscribe(OnTaskCategoryChanged);
        _eventAggregator.GetEvent<TaskItemsCategoryChangedEvent>().Subscribe(OnTasksCategoryChanged);
        
        _eventAggregator.GetEvent<TaskSortingRequestedEvent>().Subscribe(OnSortingRequested);
        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Subscribe(OnVersionRestored);
        _eventAggregator.GetEvent<TaskSplittedByLinesEvent>().Subscribe(OnTaskSplitted);
        _eventAggregator.GetEvent<TaskItemMoveToTopClickedEvent>().Subscribe(OnMoveToTopRequested);
        _eventAggregator.GetEvent<TaskItemMoveToBottomClickedEvent>().Subscribe(OnMoveToBottomRequested);
        _eventAggregator.GetEvent<TaskResetRequestedEvent>().Subscribe(OnTaskResetRequested);
        
        _eventAggregator.GetEvent<TaskDeleteAllRequestedEvent>().Subscribe(OnDeleteAllRequested);

        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Subscribe(OnTagsChangedOnTaskItem);
        _eventAggregator.GetEvent<TagItemDeletedEvent>().Subscribe(OnTagItemDeleted);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Subscribe(OnTagItemUpdated);

        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Subscribe(OnCtrlFPressed);
        _eventAggregator.GetEvent<HotkeyPressedCtrlNEvent>().Subscribe(OnCtrlNPressed);
        
        _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(OnThemeChanged);

        _oneEditorOpenService.ChangedToDisplayMode += FocusAddNewTaskTextEditor;
        SearchBoxViewModel.SearchTermsChanged += OnSearchTermsChanged;
    }

    private void UnsubscribeFromEvents()
    {
        AppSettings.Instance.PageTitleSettings.SettingsChanged -= OnPageTitleSettingsChanged;
        Items.CollectionChanged -= ItemsOnCollectionChanged;

        _eventAggregator.GetEvent<TaskItemDeleteClickedEvent>().Unsubscribe(OnDeleteTaskItemRequestedEvent);
        _eventAggregator.GetEvent<TaskItemPinClickedEvent>().Unsubscribe(OnPinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUnpinClickedEvent>().Unsubscribe(OnUnpinTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCheckedEvent>().Unsubscribe(OnFinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemUncheckedEvent>().Unsubscribe(OnUnfinishTaskItemRequested);
        _eventAggregator.GetEvent<TaskItemCategoryChangedEvent>().Unsubscribe(OnTaskCategoryChanged);
        _eventAggregator.GetEvent<TaskItemsCategoryChangedEvent>().Unsubscribe(OnTasksCategoryChanged);

        _eventAggregator.GetEvent<TaskSortingRequestedEvent>().Unsubscribe(OnSortingRequested);
        _eventAggregator.GetEvent<TaskItemVersionRestoredEvent>().Unsubscribe(OnVersionRestored);
        _eventAggregator.GetEvent<TaskSplittedByLinesEvent>().Unsubscribe(OnTaskSplitted);
        _eventAggregator.GetEvent<TaskItemMoveToTopClickedEvent>().Unsubscribe(OnMoveToTopRequested);
        _eventAggregator.GetEvent<TaskItemMoveToBottomClickedEvent>().Unsubscribe(OnMoveToBottomRequested);
        _eventAggregator.GetEvent<TaskResetRequestedEvent>().Unsubscribe(OnTaskResetRequested);

        _eventAggregator.GetEvent<TaskDeleteAllRequestedEvent>().Unsubscribe(OnDeleteAllRequested);

        _eventAggregator.GetEvent<TagsChangedOnTaskItemEvent>().Unsubscribe(OnTagsChangedOnTaskItem);
        _eventAggregator.GetEvent<TagItemDeletedEvent>().Unsubscribe(OnTagItemDeleted);
        _eventAggregator.GetEvent<TagItemUpdatedEvent>().Unsubscribe(OnTagItemUpdated);

        _eventAggregator.GetEvent<HotkeyPressedCtrlFEvent>().Unsubscribe(OnCtrlFPressed);
        _eventAggregator.GetEvent<HotkeyPressedCtrlNEvent>().Unsubscribe(OnCtrlNPressed);

        _eventAggregator.GetEvent<ThemeChangedEvent>().Unsubscribe(OnThemeChanged);

        _oneEditorOpenService.ChangedToDisplayMode -= FocusAddNewTaskTextEditor;
        SearchBoxViewModel.SearchTermsChanged -= OnSearchTermsChanged;
    }

    private void OnTextBoxFocused() => _oneEditorOpenService.EditModeWithoutTask();

    private bool FilterTaskItem(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchBoxViewModel.SearchText)) return true;

        if (obj is TaskItemViewModel taskItem)
        {
            var plainTextContent = taskItem.Content.GetContentInPlainText();

            var res = SearchBoxViewModel.SearchTerms
                .All(term => plainTextContent.Contains(term, StringComparison.OrdinalIgnoreCase));

            return res;
        }

        return false;
    }

    private void AddTaskItem()
    {
        if (!NewContentViewModel.IsEmpty)
        {
            var activeCategory = _mediator.Send(new GetSelectedCategoryQuery()).Result;
            var newListOrder = _mediator.Send(new TaskCreationListOrderQuery { CategoryId = activeCategory.Id }).Result;
            var isLastItem = newListOrder == Items.Count;

            var task = new TaskItem
            {
                Content = NewContentViewModel.GetContent(),
                ContentPreview = NewContentViewModel.GetContentInPlainText(),
                IsContentPlainText = NewContentViewModel.IsPlainTextMode,
                CategoryId = activeCategory.Id,
                ListOrder = newListOrder
            };

            var addedTask = _taskItemRepository.AddTask(task);

            _oneEditorOpenService.LastEditedTaskId = addedTask.Id;

            Items.Insert(newListOrder, addedTask.MapToViewModel(_mediator, _oneEditorOpenService, _eventAggregator));

            if (!isLastItem)
            {
                // Fix list orders
                Items.SetListOrdersToIndex();
                for (var i = 0; i < Items.Count; i++)
                {
                    Items[i].ListOrder = i;
                }

                _taskItemRepository.UpdateTaskListOrders(Items.MapList());
            }

            ScrollIntoViewRequested?.Invoke(newListOrder);
            RecalculateProgress();

            NewContentViewModel.SetContent(NewContentViewModel.IsPlainTextMode, string.Empty);
        }
    }

    private void OnQuickEditRequested()
    {
        var lastAddedTaskItem = Items.FirstOrDefault(x => x.Id == _oneEditorOpenService.LastEditedTaskId);

        // If null, then the item is not in the list.
        // The filter is checked to see if the item is visible
        if (lastAddedTaskItem != null && FilterTaskItem(lastAddedTaskItem))
        {
            int index = Items.IndexOf(lastAddedTaskItem);
            ScrollIntoViewRequested?.Invoke(index);
            lastAddedTaskItem.Cmd.EditItemCommand.Execute(null);
        }
    }

    private void EditCategory()
    {
        IsCategoryInEditMode = true;
        var activeCategory = _mediator.Send(new GetSelectedCategoryQuery()).Result;
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

    private void OnSearchTermsChanged() => ItemsView.Refresh();

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
    {
        if (_ignoreCollectionChange) return;

        FixItemsListOrders(persist: true);
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

        orderedTasks.SetListOrdersToIndex();

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

    // Event handlers below ===========

    private void OnDeleteTaskItemRequestedEvent(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        Items.Remove(taskItem);
        RecalculateProgress();

        _taskItemRepository.DeleteTask(taskItem.Map());
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
                    ? Items.OrderBy(x => x.Content.GetContentInPlainText())
                    : Items.OrderByDescending(x => x.Content.PlainTextContent);
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

        _ignoreCollectionChange = true;

        Items.Clear();
        foreach (var taskItem in sortedItems)
        {
            Items.Add(taskItem);
        }

        FixItemsListOrders(persist: true);

        _ignoreCollectionChange = false;
    }

    private void OnVersionRestored(int taskId)
    {
        var dbTask = _taskItemRepository.GetTaskById(taskId, includeNavigation: true);
        ArgumentNullException.ThrowIfNull(dbTask);

        var updatedTask = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(updatedTask);

        updatedTask.Content.SetContent(dbTask.IsContentPlainText, dbTask.Content);

        updatedTask.ModificationDate = dbTask.ModificationDate;
        updatedTask.Versions = dbTask.Versions.MapToViewModelList(_mediator);
    }

    private void OnCtrlFPressed()
    {
        SearchBoxViewModel.IsSearchBoxOpen = true;
        SearchBoxViewModel.TriggerSearchBoxFocus = true;
    }

    private void OnCtrlNPressed()
    {
        NewContentViewModel.TriggerFocus = true;
    }

    private void OnThemeChanged()
    {
        ItemsView.Refresh();
    }

    private void OnTaskSplitted(int categoryId)
    {
        var activeCategoryInfo = _mediator.Send(new GetSelectedCategoryQuery()).Result;
        if (activeCategoryInfo.Id == categoryId)
        {
            var tasks = _taskItemRepository.GetActiveTasksFromCategory(activeCategoryInfo.Id, includeNavigation: true);

            _ignoreCollectionChange = true;

            Items.Clear();

            foreach (var taskItem in tasks)
            {
                Items.Add(taskItem.MapToViewModel(_mediator, _oneEditorOpenService, _eventAggregator));
            }

            FixItemsListOrders(persist: true);

            _ignoreCollectionChange = false;
        }
    }

    private void OnMoveToTopRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        var query = new TaskDragDropInsertPositionQuery
        {
            TaskId = taskId,
            RequestedInsertPosition = 0
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnMoveToBottomRequested(int taskId)
    {
        var taskItem = Items.FirstOrDefault(x => x.Id == taskId);
        ArgumentNullException.ThrowIfNull(taskItem);

        var query = new TaskDragDropInsertPositionQuery
        {
            TaskId = taskId,
            RequestedInsertPosition = Items.Count - 1
        };

        var newIndex = _mediator.Send(query).Result;

        MoveTaskItem(newIndex, taskItem);
    }

    private void OnTaskResetRequested(TaskResetRequestedPayload payload)
    {
        foreach (var taskItem in Items)
        {
            switch (payload.ResetSubject)
            {
                case TaskResetRequestedPayload.Subject.State:
                {
                    taskItem.IsDone = false;
                    taskItem.Pinned = false;
                    break;
                }
                case TaskResetRequestedPayload.Subject.AllColors:
                {
                    taskItem.MarkerColor = Constants.ColorName.Transparent;
                    taskItem.BackgroundColor = Constants.ColorName.Transparent;
                    taskItem.BorderColor = Constants.ColorName.Transparent;
                    break;
                }
                case TaskResetRequestedPayload.Subject.MarkerColor:
                {
                    taskItem.MarkerColor = Constants.ColorName.Transparent;
                    break;
                }
                case TaskResetRequestedPayload.Subject.BackgroundColor:
                {
                    taskItem.BackgroundColor = Constants.ColorName.Transparent;
                    break;
                }
                case TaskResetRequestedPayload.Subject.BorderColor:
                {
                    taskItem.BorderColor = Constants.ColorName.Transparent;
                    break;
                }
                case TaskResetRequestedPayload.Subject.Tag:
                {
                    taskItem.Tags.Clear();
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        if (payload.ResetSubject == TaskResetRequestedPayload.Subject.Tag)
        {
            _taskItemRepository.RemoveTagsFromTasks(Items.MapList());
        }
        else
        {
            _taskItemRepository.UpdateTaskItems(Items.MapList());

            if (payload.ResetSubject == TaskResetRequestedPayload.Subject.State)
            {
                RecalculateProgress();
            }
        }
    }

    private void OnDeleteAllRequested(TaskDeleteAllRequestedPayload payload)
    {
        List<TaskItem> itemsToDelete;

        _ignoreCollectionChange = true;

        if (payload.DeleteMode == TaskDeleteAllRequestedPayload.Mode.All)
        {
            itemsToDelete = Items.MapList();
            Items.Clear();
        }
        else if (payload.DeleteMode == TaskDeleteAllRequestedPayload.Mode.Completed)
        {
            var query = Items.Where(x => x.IsDone);
            itemsToDelete = query.MapList();
            Items.RemoveAll(query.ToList());
        }
        else if (payload.DeleteMode == TaskDeleteAllRequestedPayload.Mode.Incomplete)
        {
            var query = Items.Where(x => !x.IsDone);
            itemsToDelete = query.MapList();
            Items.RemoveAll(query.ToList());
        }
        else
        {
            throw new NotImplementedException("This delete mode is not implemented!");
        }

        FixItemsListOrders(persist: true);

        _ignoreCollectionChange = false;

        _taskItemRepository.DeleteTasks(itemsToDelete);
        RecalculateProgress();
    }

    private void OnTaskCategoryChanged(TaskItemCategoryChangedPayload payload)
    {
        var activeCategory = _mediator.Send(new GetSelectedCategoryQuery()).Result;
        if (activeCategory.Id != payload.NewCategoryId)
        {
            var task = Items.FirstOrDefault(x => x.Id == payload.TaskId);
            ArgumentNullException.ThrowIfNull(task);

            _ignoreCollectionChange = true;
            
            Items.Remove(task);
            FixItemsListOrders();
            
            _ignoreCollectionChange = false;
        }
    }

    private void OnTasksCategoryChanged(TaskItemsCategoryChangedPayload payload)
    {
        var activeCategory = _mediator.Send(new GetSelectedCategoryQuery()).Result;
        if (activeCategory.Id != payload.NewCategoryId)
        {
            _ignoreCollectionChange = true;

            foreach (var taskId in payload.TaskIds)
            {
                var task = Items.FirstOrDefault(x => x.Id == taskId);
                ArgumentNullException.ThrowIfNull(task);
                Items.Remove(task);
            }

            FixItemsListOrders();

            _ignoreCollectionChange = false;
        }
    }

    private void OnTagItemDeleted(int tagId)
    {
        // Remove deleted tag from every tasks
        foreach (var item in Items)
        {
            var tag = item.Tags.FirstOrDefault(x => x.Id == tagId);
            if (tag != null)
            {
                item.Tags.Remove(tag);
            }
        }
    }

    private void OnTagItemUpdated(int tagId)
    {
        var tag = _taskItemRepository.GetTagById(tagId);
        ArgumentNullException.ThrowIfNull(tag);

        foreach (var taskItem in Items)
        {
            var tagOnTask = taskItem.Tags.FirstOrDefault(x => x.Id == tagId);
            if (tagOnTask != null)
            {
                tagOnTask.Color = EnumHelper.ConvertTo<TagColor>(tag.Color);
                tagOnTask.Name = tag.Name;
            }
        }
    }

    private void FixItemsListOrders(bool persist = false)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
            Items[i].IsFirstItem = i == 0;
        }

        if (persist)
        {
            _taskItemRepository.UpdateTaskListOrders(Items.MapList());
        }
    }

    protected override void OnDispose() => UnsubscribeFromEvents();
}