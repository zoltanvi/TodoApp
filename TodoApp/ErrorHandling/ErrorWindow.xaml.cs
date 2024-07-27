using System.Windows;

namespace TodoApp.ErrorHandling;

public partial class ErrorWindow : Window
{
    public ErrorWindow(string title, string errorMessage)
    {
        InitializeComponent();
        DataContext = new ErrorWindowViewModel(title, errorMessage);
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
