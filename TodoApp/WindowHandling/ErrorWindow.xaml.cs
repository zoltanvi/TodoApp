using System.Windows;
using Modules.Common.ViewModel;
using PropertyChanged;

namespace TodoApp.WindowHandling;

/// <summary>
/// Interaction logic for ErrorWindow.xaml
/// </summary>
public partial class ErrorWindow : Window
{
    private ErrorWindowViewModel _viewModel;

    public ErrorWindow(string title, string errorMessage)
    {
        InitializeComponent();
        _viewModel = new ErrorWindowViewModel(title, errorMessage);
        DataContext = _viewModel;
    }

    public void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

[AddINotifyPropertyChangedInterface]
public class ErrorWindowViewModel : BaseViewModel
{
    public ErrorWindowViewModel(string title, string errorMessage)
    {
        Title = title;
        ErrorMessage = errorMessage;
    }

    public string Title { get; set; }
    public string ErrorMessage { get; set; }
}
