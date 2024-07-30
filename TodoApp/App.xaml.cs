using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Common;
using Modules.Common.Cqrs.Events;
using Modules.Common.Database;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Services;
using Modules.PopupMessage.Views;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Views.Services;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using TodoApp.ErrorHandling;
using Application = System.Windows.Application;

namespace TodoApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// 
public partial class App : Application
{
    private readonly IHost _host;
    private IServiceProvider ServiceProvider => _host.Services;

    public App()
    {
        // Subscribe to exception handling
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("appsettings.Dev.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureAppServices();
            })
            .Build();

        ServiceLocator.ServiceProvider = ServiceProvider;
        ServiceProvider.InitializeDatabase();
        PublishApplicationOpeningEvent();

        var autoSaveService = ServiceProvider.GetRequiredService<IAppSettingsAutoSaveService>();
        autoSaveService.StartService();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set app version info
        var version = (string)Current.TryFindResource(Constants.CurrentVersion);
        AppSettings.Instance.ApplicationSettings.AppVersion = version;

        CreateMainWindow();
    }

    private void PublishApplicationOpeningEvent()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        mediator.Publish(new ApplicationOpeningEvent());
    }

    private void CreateMainWindow()
    {
        // Show the main window
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        var mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = mainWindowViewModel;

        var messageControl = ServiceProvider.GetRequiredService<PopupMessageControl>();
        mainWindow.MessageLineGrid.Children.Add(messageControl);

        mainWindow.Show();
        Current.MainWindow = mainWindow;

        var mainPageNavigation = ServiceProvider.GetRequiredService<IMainPageNavigationService>();
        mainPageNavigation.Initialize(mainWindow.MainFrame);

        var sideMenuPageNavigation = ServiceProvider.GetRequiredService<ISideMenuPageNavigationService>();
        sideMenuPageNavigation.Initialize(mainWindow.SideMenuFrame);

        var overlayPageNavigation = ServiceProvider.GetRequiredService<IOverlayPageNavigationService>();
        overlayPageNavigation.Initialize(mainWindow.OverlayFrame);
        overlayPageNavigation.InitializeOverlayElements(mainWindow.OverlayBackground, mainWindow.OverlayFrameGrid);

        if (AppSettings.Instance.SessionSettings.ActiveCategoryId == Constants.RecycleBinCategoryId)
        {
            mainPageNavigation.NavigateTo<IRecycleBinPage>();
        }
        else
        {
            mainPageNavigation.NavigateTo<ITaskPage>();
        }

        sideMenuPageNavigation.NavigateTo<ICategoryListPage>();
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) => LogException(e.Exception);
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) => LogException(e.Exception.InnerException);
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) => LogException(e.ExceptionObject as Exception);

    private void LogException(Exception? ex)
    {
        if (ex == null) return;

        var dbFolder = Path.GetDirectoryName(DbConfiguration.DatabasePath);
        var reportFolder = Path.Combine(dbFolder, "CrashReports");
        Directory.CreateDirectory(reportFolder);

        var fileName = $"{DateTime.Now:yyyy-MM-dd__HH_mm_ss_ffff}.txt";
        var reportFilePath = Path.Combine(reportFolder, fileName);

        StringBuilder sb = new StringBuilder();
        while (ex != null)
        {
            sb.Append(DateTime.Now.ToLongDateString() + "  ");
            sb.AppendLine(DateTime.Now.ToLongTimeString());
            sb.AppendLine(ex.Message);
            sb.AppendLine();
            sb.AppendLine(ex.StackTrace);
            sb.AppendLine();
            sb.AppendLine();

            ex = ex.InnerException;
        }

        var errorDetails = sb.ToString();
        File.AppendAllText(reportFilePath, errorDetails);

        var window = new ErrorWindow("An error occurred.", errorDetails);
        window.ShowDialog();
    }
}

// Class to jump to for quick navigation with resharper
public class Init;

