using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Tasks.Views.Pages;

public partial class TagSelectorPage : GenericBasePage<TagSelectorPageViewModel>, ITagSelectorPage
{
    public TagSelectorPage(TagSelectorPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}