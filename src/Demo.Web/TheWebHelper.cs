using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Web
{
    public class TheWebHelper
    {
        public static TheWebHelper Instance = new TheWebHelper();

        public IServiceProvider RootServiceProvider { get; set; }

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
    }
}
