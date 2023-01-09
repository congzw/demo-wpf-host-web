using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Common.Workers
{
    public interface ILoopWorkerHelper
    {
        LoopWorkerSetting GetSetting();
        void SetSetting(LoopWorkerSetting setting);

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task ExecuteAsync(CancellationToken stoppingToken);

        Task<bool> ShouldDoWorkAsync();
        Task DoWorkAsync(CancellationToken cancellationToken);

        void SetLogger(ILogger logger);
        void Log(object msg);
    }

    public class LoopWorkerSetting
    {
        public bool Disabled { get; set; } = false;
        public int LoopSeconds { get; set; } = LoopSecondsDefault;
        public TimeSpan GetLoopSpan() => TimeSpan.FromSeconds(LoopSeconds);

        public static int LoopSecondsDefault = 5;
        public static void AutoFix(LoopWorkerSetting setting, bool fixNull = true)
        {
            if (setting == null)
            {
                if (fixNull)
                {
                    setting = new LoopWorkerSetting();
                }
            }

            if (setting.LoopSeconds <= 0)
            {
                setting.LoopSeconds = LoopSecondsDefault;
            }
        }
    }

    public abstract class BaseLoopWorkerHelper<T> : ILoopWorkerHelper where T : ILoopWorkerHelper
    {
        protected LoopWorkerSetting Setting { get; set; } = new LoopWorkerSetting();

        public LoopWorkerSetting GetSetting()
        {
            LoopWorkerSetting.AutoFix(Setting);
            return Setting;
        }
        public void SetSetting(LoopWorkerSetting setting)
        {
            if (setting is null)
            {
                throw new ArgumentNullException(nameof(setting));
            }
            LoopWorkerSetting.AutoFix(setting);
            Setting = setting;
            Log($"SaveSetting => Disabled:{Setting.Disabled} LoopSpan:{Setting.GetLoopSpan()}");
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            Log($"StartAsync => Disabled:{Setting.Disabled} LoopSpan:{Setting.GetLoopSpan()}");
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            Log($"StopAsync => Disabled:{Setting.Disabled} LoopSpan:{Setting.GetLoopSpan()}");
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Setting.Disabled)
                {
                    var shouldDoWork = await ShouldDoWorkAsync();
                    if (shouldDoWork)
                    {
                        await DoWorkAsync(stoppingToken);
                    }
                }
                await Task.Delay(Setting.GetLoopSpan(), stoppingToken);
            }
        }

        public virtual async Task<bool> ShouldDoWorkAsync()
        {
            await Task.CompletedTask;
            if (Setting.Disabled)
            {
                return false;
            }
            return true;
        }
        public abstract Task DoWorkAsync(CancellationToken cancellationToken);

        protected ILogger InnerLogger { get; set; }
        public void SetLogger(ILogger logger)
        {
            InnerLogger = logger;
        }
        public virtual void Log(object msg)
        {
            if (InnerLogger != null)
            {
                InnerLogger.LogInformation($" >>> [{GetType().Name} {GetHashCode()}] {DateTimeOffset.Now:F} => {msg}");
                return;
            }
            //todo: memory log queue helper
            Console.WriteLine($" >>> [{GetType().Name} {GetHashCode()}] {DateTimeOffset.Now:F} => {msg}");
        }
    }

    public abstract class BaseLoopWorker<T> : BackgroundService where T : ILoopWorkerHelper
    {
        private readonly T _loopWorkerHelper;

        public BaseLoopWorker(T loopWorkerHelper, ILogger<T> logger)
        {
            _loopWorkerHelper = loopWorkerHelper;
            _loopWorkerHelper.SetLogger(logger);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _loopWorkerHelper.ExecuteAsync(stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _loopWorkerHelper.StartAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _loopWorkerHelper.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }

    public class LoopWorker<T> : BaseLoopWorker<T> where T : ILoopWorkerHelper
    {
        public LoopWorker(T loopWorkerHelper, ILogger<T> logger) : base(loopWorkerHelper, logger)
        {
        }
    }

    namespace Demo
    {
        public class DemoLoopWorkerHelper : BaseLoopWorkerHelper<DemoLoopWorkerHelper>
        {
            public override async Task DoWorkAsync(CancellationToken cancellationToken)
            {
                Log("do some work...");
                await Task.Delay(500);
            }

            public override async Task StartAsync(CancellationToken cancellationToken)
            {
                Log("run start logic...");
                await base.StartAsync(cancellationToken);
            }

            public override async Task StopAsync(CancellationToken cancellationToken)
            {
                Log("run stop logic...");
                await base.StopAsync(cancellationToken);
            }
        }

        public class DemoLoopWorkerHelper2 : BaseLoopWorkerHelper<DemoLoopWorkerHelper>
        {
            public DemoLoopWorkerHelper2()
            {
                Setting.LoopSeconds = 1;
                Setting.Disabled = true;
            }

            public override async Task DoWorkAsync(CancellationToken cancellationToken)
            {
                Log("do some work2...");
                await Task.Delay(1000);
            }

            public override async Task StartAsync(CancellationToken cancellationToken)
            {
                Log("run start logic2...");
                await base.StartAsync(cancellationToken);
            }

            public override async Task StopAsync(CancellationToken cancellationToken)
            {
                Log("run stop logic2...");
                await base.StopAsync(cancellationToken);
            }
        }

        public static class DemoLoopWorkerStartup
        {
            public static IServiceCollection AddTheDemoLoopWorker(this IServiceCollection services)
            {
                services.AddSingleton<DemoLoopWorkerHelper>();
                services.AddHostedService<LoopWorker<DemoLoopWorkerHelper>>();
                services.AddSingleton<DemoLoopWorkerHelper2>();
                services.AddHostedService<LoopWorker<DemoLoopWorkerHelper2>>();
                return services;
            }
        }
    }
}