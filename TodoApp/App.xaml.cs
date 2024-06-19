using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Settings.Repositories;
using System.Windows;
using Modules.Migration;
using MediatR;
using Modules.Common.Cqrs.Events;
using Microsoft.Extensions.Configuration;
using Modules.Common.Database;

namespace TodoApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// 
public partial class App : Application
{
    private IHost _host;

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

        InitializeDatabase();
        PublishApplicationOpeningEvent();

    }

    private void InitializeDatabase()
    {
        DbConfiguration.Initialize(_host.Services.GetRequiredService<IConfiguration>());

        var migrationService = _host.Services.GetService<IMigrationService>();

        var dbContextList = new List<DbContext>
        {
            _host.Services.GetService<SettingDbContext>(),
        };

        migrationService.Run(dbContextList);
    }

    private void PublishApplicationOpeningEvent()
    {
        var mediator = _host.Services.GetService<IMediator>();
        mediator.Publish(new ApplicationOpeningEvent());
    }
}

