using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Workers
{
    public class SimpleWorker : BackgroundService
    {
        private bool _shouldStop = false;
        private readonly ILogger<SimpleWorker> _logger;
        private readonly SimpleWorkerHelper _simpleWorkerHelper;

        public SimpleWorker(ILogger<SimpleWorker> logger, SimpleWorkerHelper simpleWorkerHelper)
        {
            _logger = logger;
            _simpleWorkerHelper = simpleWorkerHelper;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Log("StartAsync");
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Log("ExecuteAsync");
                // 这里实现实际的业务逻辑
                while (true)
                {
                    if (_shouldStop)
                    {
                        Log("should stop working");
                        break;
                    }

                    if (stoppingToken.IsCancellationRequested)
                    {
                        Log("stoppingToken IsCancellationRequested");
                        break;
                    }

                    if (!_simpleWorkerHelper.ShouldWorking)
                    {
                        Log("wait for working command!");
                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                        continue;
                    }
                    await DoTheWork(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Log("ex: " + ex.Message);
            }
            finally
            {
                Log("exit looping!");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log("StopAsync");
            _shouldStop = true;
            await DoTheClean();
            Log("StopAsync >>>");
            await base.StopAsync(cancellationToken);
        }

        private async Task DoTheWork(CancellationToken cancellationToken)
        {
            _simpleWorkerHelper.AddExecutedCount();
            Log("Working... => " + _simpleWorkerHelper.ExecutedCount);
            await Task.CompletedTask;
        }

        private async Task DoTheClean()
        {
            //mock some clean works
            for (int i = 0; i < 10; i++)
            {
                Log("DoTheClean => " + (10 - i));
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
            await Task.CompletedTask;
        }

        private void Log(string msg, LogLevel logLevel = LogLevel.Information)
        {
            var threadInfo = $"";
            _logger.Log(logLevel, ">>> " + msg + threadInfo);
        }
    }

    public class SimpleWorkerHelper
    {
        private long executedCount;

        public long ExecutedCount { get => executedCount; private set => executedCount = value; }
        public bool ShouldWorking { get; set; }

        public void AddExecutedCount()
        {
            executedCount = Interlocked.Increment(ref executedCount);
        }
    }
}
