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
using System.Threading.Tasks;

namespace WpfApp
{
    public partial class App : Application
    {
        private IHost _host;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App()
        {
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
            .ConfigureLogging((context, logging) =>
            {
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                logging.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            })
            .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LogTitle("App.OnStartup", false);

            Log("host Start");
            _host.Start();
            Log("host Start >>>");
            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Host = _host;
            mainWindow.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            //this method cannot be async, it MUST wait!
            using (_host)
            {
                Log("host StopAsync");
                //给予一个最大的等待时间长度，来清理和结束后台任务
                Task.Run(async () => await _host.StopAsync()).Wait(TimeSpan.FromSeconds(30));
                Log("host StopAsync >>>");
            }

            LogTitle("App.OnExit", true);
            base.OnExit(e);
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
