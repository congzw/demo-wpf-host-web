using Demo.Web;
using Demo.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Demo.Wpf
{
    public partial class App : Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider _serviceProvider;
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
              .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TheStartup>();
            })
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddTheAppServices();
              })
              .Build();

            _serviceProvider = _host.Services;
            //SimpleServiceLocator.Instance.SetProvider(_serviceProvider);
            GlobalExceptionHandle();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LogTitle("App.OnStartup", false);

            Log("host Start");
            _host.Start();
            Log("host Start >>>");

            using (var scope = _serviceProvider.CreateScope())
            {
                var theLogger = scope.ServiceProvider.GetRequiredService<ILogger<WpfAppControl>>();
                var wpfAppControl = scope.ServiceProvider.GetRequiredService<WpfAppControl>();
                wpfAppControl.Logger = theLogger;
                wpfAppControl.Start();

                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                WinControlService.TheMainWindow = mainWindow;
                mainWindow.Show();

                //var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                //var wpfAppControl = scope.ServiceProvider.GetRequiredService<WpfAppControl>();
                //wpfAppControl.Logger = theLogger;
                //wpfAppControl.TheMainWindow = mainWindow;
                //wpfAppControl.Start();
                //mainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //this method cannot be async, it MUST wait!
            using (_host)
            {
                Log("host StopAsync");
                //Task.Run(async () => await _host.StopAsync()).Wait();
                //如果后台的清理任务超过某个值，也强制退出
                Task.Run(async () => await _host.StopAsync()).Wait(TimeSpan.FromSeconds(30));
                Log("host StopAsync >>>");
            }

            LogTitle("App.OnExit", true);
            LogManager.Shutdown();
            base.OnExit(e);
        }

        private void GlobalExceptionHandle()
        {
            //UI线程未捕获异常处理事件 
            this.DispatcherUnhandledException += (sender, args) =>
            {
                args.Handled = true;
                LogError(args.Exception, "UI线程异常[DispatcherUnhandledException]");
            };
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                args.SetObserved();
                LogError(args.Exception, "Task线程异常[UnobservedTaskExceptionEventArgs]");
            };
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception e)
                    LogError(e, $"非UI线程异常[UnhandledExceptionEventArgs] IsTerminating ==> {args.IsTerminating}");
                else
                    LogError(new Exception(args.ExceptionObject.ToString()), $"非UI线程异常[UnhandledExceptionEventArgs] IsTerminating ==> {args.IsTerminating}");
            };
        }

        private void Log(string msg)
        {
            _logger.Info(msg);
        }

        private void LogError(Exception ex, string exMsg)
        {
            _logger.Error(ex, exMsg);
        }

        private void LogTitle(string msg, bool appendNewLine = false)
        {
            var append = appendNewLine ? Environment.NewLine : string.Empty;
            _logger.Info($"#################################");
            _logger.Info($"       {msg}");
            _logger.Info($"#################################{append}");
        }
    }

    public class FooService2 : IFooService2
    {
        public string GetDesc()
        {
            return $"{this.GetType().FullName} - {this.GetHashCode()}";
        }
    }
}
