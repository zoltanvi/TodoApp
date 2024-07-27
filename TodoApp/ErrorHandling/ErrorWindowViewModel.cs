using Modules.Common.ViewModel;
using PropertyChanged;

namespace TodoApp.ErrorHandling;

[AddINotifyPropertyChangedInterface]
public class ErrorWindowViewModel(string title, string errorMessage) : BaseViewModel
{
    public string Title { get; set; } = title;
    public string ErrorMessage { get; set; } = errorMessage;
}
