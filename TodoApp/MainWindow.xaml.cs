using Modules.Common.Services;
using Modules.Settings.Contracts.ViewModels;
using Prism.Events;
using System.Windows;
using TodoApp.WindowHandling;

namespace TodoApp;

public partial class MainWindow : Window
{
    private GridResizer _gridResizer;

    public MainWindow(IUIScaler uiScaler, IEventAggregator eventAggregator, IAppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(appSettings);

        InitializeComponent();

        _gridResizer = new GridResizer(Grid, Resizer, this, uiScaler, eventAggregator, appSettings);
    }
}