using Modules.Common.ViewModel;
using Modules.Tasks.TextEditor.Controls;
using PropertyChanged;

namespace Modules.RecycleBin.Views.Controls;

// TODO:
[AddINotifyPropertyChangedInterface]
public class RecycleBinTaskItemVersionViewModel : BaseViewModel
{
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public DynamicTextBoxViewModel Content { get; set; }

    public DateTime VersionDate { get; set; }

    public RecycleBinTaskItemVersionViewModel(string content, bool isContentPlainText)
    {
        Content = new DynamicTextBoxViewModel();

        Content.SetContent(isContentPlainText, content);
    }
}
