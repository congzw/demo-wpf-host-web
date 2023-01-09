using Demo.Web;
using Demo.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        protected override async void OnStartup(StartupEventArgs e)
        {
            LogTitle("Main Startup", true);

            await _host.StartAsync();

            using (var scope = _serviceProvider.CreateScope())
            {
                //启动主窗体
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Host = _host;
                mainWindow.Show();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            //using (_host)
            //{
            //    //Microsoft.Hosting.Lifetime: Information: Waiting for the host to be disposed.Ensure all 'IHost' instances are wrapped in 'using' blocks.
            //    //await _host.StopAsync();
            //    //using (var scope = _serviceProvider.CreateScope())
            //    //{
            //    //    var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
            //    //    lifetime.StopApplication();
            //    //}
            //    //await _host.StopAsync();
            //}

            LogTitle("Main Exit", true);
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
