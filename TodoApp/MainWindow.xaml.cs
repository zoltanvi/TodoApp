using Modules.Common.Services;
using System.Windows;
using TodoApp.WindowHandling;

namespace TodoApp;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GridResizer _gridResizer;

    public MainWindow(IUIScaler uiScaler)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);

        InitializeComponent();

        _gridResizer = new GridResizer(Grid, Resizer, this, uiScaler);
    }
}