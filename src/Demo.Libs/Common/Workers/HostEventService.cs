using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Common.Workers
{
    public class HostEventService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<HostEventService> _logger;

        public HostEventService(ILogger<HostEventService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;

            var theName = _hostApplicationLifetime.GetType().Name;

            _hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                Log($"{theName}.ApplicationStarted");

                var osInfo = OsInfo.Create();
                Log($"#################################");
                Log($"    OsInfo    ");
                Log($"OSDescription: {osInfo.OSDescription}");
                Log($"RuntimeIdentifier: {osInfo.RuntimeIdentifier}");
                Log($"OSArchitecture: {osInfo.OSArchitecture}");
                Log($"ProcessArchitecture: {osInfo.ProcessArchitecture}");
                Log($"OSName: {osInfo.OSName}");
                Log($"################################# {Environment.NewLine}");

                //todo
            });

            _hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                Log($"{theName}.ApplicationStopping");
                //todo
            });

            _hostApplicationLifetime.ApplicationStopped.Register(() =>
            {
                Log($"{theName}.ApplicationStopped");
                //todo
            });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log("ExecuteAsync");
            return Task.CompletedTask;
        }

        private void Log(string msg)
        {
            _logger.LogDebug(msg);
            //_logger.LogInformation(msg);
        }
    }

    public class OsInfo
    {
        public string OSDescription { get; set; }
        public string RuntimeIdentifier { get; set; }
        public Architecture OSArchitecture { get; set; }
        public Architecture ProcessArchitecture { get; set; }
        public string OSName { get; set; }

        public static OsInfo Create()
        {
            var info = new OsInfo();
            info.OSDescription = RuntimeInformation.OSDescription;
            info.RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier;
            info.OSArchitecture = RuntimeInformation.OSArchitecture;
            info.ProcessArchitecture = RuntimeInformation.ProcessArchitecture;
            info.OSName = "UnknownOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.OSName = OSPlatform.Windows.ToString();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info.OSName = OSPlatform.Windows.ToString();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                info.OSName = OSPlatform.Linux.ToString();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                info.OSName = OSPlatform.OSX.ToString();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                info.OSName = OSPlatform.FreeBSD.ToString();
            }
            return info;
        }
    }
}
