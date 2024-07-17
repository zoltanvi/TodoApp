using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.RecycleBin.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class RecycleBinTaskItemVersionViewModel : BaseViewModel
{
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public required string Content { get; set; }
    public required string ContentPreview { get; set; }
    public DateTime VersionDate { get; set; }
}
