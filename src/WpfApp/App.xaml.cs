using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Windows;
using NLog.Extensions.Logging;
using NLog;
using WpfApp.Workers;
using NLog.Fluent;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            LogTitle("Main App Ctor", true);

            this.Startup += App_Startup;
            this.Exit += App_Exit;

            //_host = new HostBuilder().Build();
            _host = new HostBuilder()
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                configurationBuilder.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<SimpleWorkerHelper>();
                services.AddHostedService<SimpleWorker>();
                services.AddSingleton<MainWindow>();
            })
            .ConfigureLogging((context, logging) => {
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                logging.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            })
            .Build();
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            LogTitle("Main App_Startup", true);
            await _host.StartAsync();
            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }

        private async void App_Exit(object sender, ExitEventArgs e)
        {
            LogTitle("Main App_Exit BEGIN", true);
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            LogTitle("Main App_Exit END", true);
        }

        private void Log(string msg)
        {
            _logger.Info(msg);
        }

        private void LogTitle(string msg, bool appendNewLine = false)
        {
            var append = appendNewLine ? Environment.NewLine : string.Empty;
            _logger.Info($"#################################");
            _logger.Info($"       {msg}");
            _logger.Info($"#################################{append}");
        }
    }
}
