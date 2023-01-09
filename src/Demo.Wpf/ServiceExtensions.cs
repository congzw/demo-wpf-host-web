using Demo.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Demo.Wpf
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddTheAppServices(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });

            services.AddSingleton<MainWindow>();
            //services.AddHostedService<TheWebHost>();

            services.AddTransient<IFooService1, FooService1>();
            services.AddSingleton<IFooService2, FooService2>();

            //services.AddTheNlog();
            //services.AddSingleton<SettingWindow>();
            //services.AddSingleton<PlayCtrlWindow>();
            //services.AddWorkers();
            //services.AddTransient<IDemoSettingService, DemoSettingService>();

            return services;
        }
    }
}