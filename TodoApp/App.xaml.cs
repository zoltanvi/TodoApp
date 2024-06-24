using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Common.Cqrs.Events;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.PopupMessage.Views;
using Modules.Settings.Views.Services;
using System.Windows;
using Application = System.Windows.Application;

namespace TodoApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// 
public partial class App : Application
{
    private IHost _host;
    private IServiceProvider ServiceProvider => _host.Services;

    public App()
    {
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

        ServiceProvider.InitializeDatabase();
        PublishApplicationOpeningEvent();

        var autoSaveService = ServiceProvider.GetRequiredService<IAppSettingsAutoSaveService>();
        autoSaveService.StartService();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        CreateMainWindow();
    }

    private void PublishApplicationOpeningEvent()
    {
        var mediator = ServiceProvider.GetService<IMediator>();
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

        var mainPageNavigation = ServiceProvider.GetService<IMainPageNavigationService>();
        ArgumentNullException.ThrowIfNull(mainPageNavigation);
        mainPageNavigation.Initialize(mainWindow.MainFrame);

        var sideMenuPageNavigation = ServiceProvider.GetService<ISideMenuPageNavigationService>();
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigation);
        sideMenuPageNavigation.Initialize(mainWindow.SideMenuFrame);

        //var overlayPageNavigation = ServiceProvider.GetService<IOverlayPageNavigationService>();
        //overlayPageNavigation.Initialize(mainWindow.OverlayBackground.OverlayFrame);

        //IoC.AppViewModel.UpdateMainPage();
        //IoC.AppViewModel.UpdateSideMenuPage();

        // Test
        //sideMenuPageNavigation.NavigateTo<ISettingsPage>();
        mainPageNavigation.NavigateTo<ISettingsPage>();
        sideMenuPageNavigation.NavigateTo<ICategoryListPage>();
    }
}

// Class to jump to for quick navigation with resharper
public class Init;

