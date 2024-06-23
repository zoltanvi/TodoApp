using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Categories.Views.Pages;

public partial class CategoryListPage : GenericBasePage<CategoryListPageViewModel>, ICategoryListPage
{
    public CategoryListPage(CategoryListPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}