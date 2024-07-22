using Modules.Common.Services;
using Prism.Events;
using System.Windows;
using TodoApp.WindowHandling;

namespace TodoApp;

public partial class MainWindow : Window
{
    private GridResizer _gridResizer;

    public MainWindow(IUIScaler uiScaler, IEventAggregator eventAggregator)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(eventAggregator);

        InitializeComponent();

        _gridResizer = new GridResizer(Grid, Resizer, this, uiScaler, eventAggregator);
    }
}