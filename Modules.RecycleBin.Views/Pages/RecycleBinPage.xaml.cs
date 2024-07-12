using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.RecycleBin.Views.Pages;

public partial class RecycleBinPage : GenericBasePage<RecycleBinPageViewModel>, IRecycleBinPage
{
    public RecycleBinPage(RecycleBinPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
