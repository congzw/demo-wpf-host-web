using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Web
{
    public class WebManager : IDisposable
    {
        public static WebManager Instance = new WebManager();
        public IServiceProvider RootServiceProvider { get; set; }

        private IHost _host;
        
        public void OpenBrowserIf()
        {
            if (RootServiceProvider == null)
            {
                return;
            }

            using (var serviceScope = RootServiceProvider.CreateScope())
            {
                var config = serviceScope.ServiceProvider.GetService<IConfiguration>();
                var url = config.GetSection("AutoOpen")?.Value;
                if (string.IsNullOrWhiteSpace(url))
                {
                    return;
                }
            
                if (string.IsNullOrWhiteSpace(url))
                {
                    return;
                }

                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    //else
                    //{
                    //    throw;
                    //}
                }
            }
        }

        public void Start()
        {
            if (_host != null)
            {
                return;
            }

            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>();
                })
                .Build();
            _host.Start();
        }

        public void Close()
        {
            _host.Dispose();
            _host = null;
        }

        public void Dispose()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }
    }
}
