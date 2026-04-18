using Modules.Categories.Contracts.Events;
using Modules.Common.Events;
using Modules.Tasks.Contracts.Events;
using Modules.Tasks.Services.Events;
using Modules.Tasks.Views.Events;

namespace Modules.Tasks.Views.Pages;

public partial class TaskPageViewModel
{
    private void SubscribeToTaskPageEvents()
    {
        _appSettings.PageTitleSettings.SettingsChanged += OnPageTitleSettingsChanged;
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
        _eventAggregator.GetEvent<FocusTaskPageNewTaskEditorEvent>().Subscribe(OnFocusTaskPageNewTaskEditor);

        _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(OnThemeChanged);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Subscribe(OnCategoryNameUpdated);

        _oneEditorOpenService.ChangedToDisplayMode += FocusAddNewTaskTextEditor;
        SearchBoxViewModel.SearchTermsChanged += OnSearchTermsChanged;
    }

    private void UnsubscribeFromTaskPageEvents()
    {
        _appSettings.PageTitleSettings.SettingsChanged -= OnPageTitleSettingsChanged;
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
        _eventAggregator.GetEvent<FocusTaskPageNewTaskEditorEvent>().Unsubscribe(OnFocusTaskPageNewTaskEditor);

        _eventAggregator.GetEvent<ThemeChangedEvent>().Unsubscribe(OnThemeChanged);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Unsubscribe(OnCategoryNameUpdated);

        _oneEditorOpenService.ChangedToDisplayMode -= FocusAddNewTaskTextEditor;
        SearchBoxViewModel.SearchTermsChanged -= OnSearchTermsChanged;
    }
}
