using Modules.Common.Navigation;
using Modules.Common.Views.Pages;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Categories.Views.Pages;

public partial class CategoryPage : GenericBasePage<CategoryPageViewModel>, ICategoryListPage
{
    public CategoryPage(CategoryPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    private void OnCategoryListPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox) return;

        var vm = ViewModel;
        var items = vm.FlattenedItems;
        if (items.Count == 0) return;

        var focusedIndex = vm.GetFocusedIndex();

        switch (e.Key)
        {
            case Key.Down:
                if (focusedIndex < items.Count - 1)
                {
                    vm.FocusedCategoryId = items[focusedIndex + 1].Id;
                }
                else if (focusedIndex == -1)
                {
                    vm.FocusedCategoryId = items[0].Id;
                }
                ScrollToFocused();
                e.Handled = true;
                break;

            case Key.Up:
                if (focusedIndex > 0)
                {
                    vm.FocusedCategoryId = items[focusedIndex - 1].Id;
                }
                else if (focusedIndex == -1)
                {
                    vm.FocusedCategoryId = items[^1].Id;
                }
                ScrollToFocused();
                e.Handled = true;
                break;

            case Key.Right:
                if (focusedIndex >= 0)
                {
                    var item = items[focusedIndex];
                    if (item.HasChildren && !item.IsExpanded)
                    {
                        vm.ExpandCategory(item.Id);
                    }
                    else if (item.HasChildren && item.IsExpanded && focusedIndex < items.Count - 1)
                    {
                        vm.FocusedCategoryId = items[focusedIndex + 1].Id;
                        ScrollToFocused();
                    }
                }
                e.Handled = true;
                break;

            case Key.Left:
                if (focusedIndex >= 0)
                {
                    var item = items[focusedIndex];
                    if (item.IsExpanded && item.HasChildren)
                    {
                        vm.CollapseCategory(item.Id);
                    }
                    else if (item.ParentCategoryId != null)
                    {
                        vm.FocusedCategoryId = item.ParentCategoryId.Value;
                        ScrollToFocused();
                    }
                }
                e.Handled = true;
                break;

            case Key.Enter:
            case Key.Space:
                if (focusedIndex >= 0)
                {
                    vm.ActivateCategory(items[focusedIndex].Id);
                }
                e.Handled = true;
                break;

            case Key.F2:
                if (focusedIndex >= 0)
                {
                    var item = items[focusedIndex];
                    item.RenameText = item.Name;
                    item.IsRenaming = true;
                }
                e.Handled = true;
                break;
        }
    }

    private void ScrollToFocused()
    {
        var vm = ViewModel;
        var index = vm.GetFocusedIndex();
        if (index >= 0 && index < vm.FlattenedItems.Count)
        {
            CategoryListListView.ScrollIntoView(vm.FlattenedItems[index]);
        }
    }
}