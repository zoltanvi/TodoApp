using Modules.Categories.Contracts.Events;
using Modules.Categories.Views.Events;
using Modules.Common.Events;

namespace Modules.Categories.Views.Pages;

public partial class CategoryPageViewModel
{
    private void SubscribeToPrismEvents()
    {
        _eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Subscribe(DeleteCategory);
        _eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(SetActiveCategory);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Subscribe(OnCategoryNameUpdated);
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Subscribe(OnCategoryRestored);
        _eventAggregator.GetEvent<CategoryToggleExpandEvent>().Subscribe(OnToggleExpand);
        _eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Subscribe(OnAddSubcategory);
        _eventAggregator.GetEvent<HotkeyPressedCtrlShiftNEvent>().Subscribe(OnHotkeyAddSubcategory);
        _eventAggregator.GetEvent<CategoryRenameClickedEvent>().Subscribe(OnRenameClicked);
        _eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Subscribe(OnMoveToRoot);
        _eventAggregator.GetEvent<CategoryMovedEvent>().Subscribe(OnCategoryMoved);
        _eventAggregator.GetEvent<CategoryMakeSubcategoryClickedEvent>().Subscribe(OnMakeSubcategory);
    }

    private void UnsubscribeFromPrismEvents()
    {
        _eventAggregator.GetEvent<CategoryDeleteClickedEvent>().Unsubscribe(DeleteCategory);
        _eventAggregator.GetEvent<CategoryClickedEvent>().Unsubscribe(SetActiveCategory);
        _eventAggregator.GetEvent<CategoryNameUpdatedEvent>().Unsubscribe(OnCategoryNameUpdated);
        _eventAggregator.GetEvent<CategoryRestoredEvent>().Unsubscribe(OnCategoryRestored);
        _eventAggregator.GetEvent<CategoryToggleExpandEvent>().Unsubscribe(OnToggleExpand);
        _eventAggregator.GetEvent<CategoryAddSubcategoryClickedEvent>().Unsubscribe(OnAddSubcategory);
        _eventAggregator.GetEvent<HotkeyPressedCtrlShiftNEvent>().Unsubscribe(OnHotkeyAddSubcategory);
        _eventAggregator.GetEvent<CategoryRenameClickedEvent>().Unsubscribe(OnRenameClicked);
        _eventAggregator.GetEvent<CategoryMoveToRootClickedEvent>().Unsubscribe(OnMoveToRoot);
        _eventAggregator.GetEvent<CategoryMovedEvent>().Unsubscribe(OnCategoryMoved);
        _eventAggregator.GetEvent<CategoryMakeSubcategoryClickedEvent>().Unsubscribe(OnMakeSubcategory);
    }
}
