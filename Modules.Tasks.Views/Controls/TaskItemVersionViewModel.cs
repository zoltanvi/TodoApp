using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Tasks.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class TaskItemVersionViewModel : BaseViewModel
{
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public required string Content { get; set; }
    public required string ContentPreview { get; set; }
    public DateTime VersionDate { get; set; }
}
