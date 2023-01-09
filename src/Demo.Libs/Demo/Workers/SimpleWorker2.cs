//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Demo.Workers
//{
//    public class SimpleWorker : BackgroundService
//    {
//        private bool _shouldStop = false;
//        private bool _cleanCompleted = false;
//        private readonly ILogger<SimpleWorker> _logger;
//        private readonly SimpleWorkerHelper _simpleWorkerHelper;
//        private readonly IHostApplicationLifetime _hostApplicationLifetime;

//        public SimpleWorker(ILogger<SimpleWorker> logger, SimpleWorkerHelper simpleWorkerHelper, IHostApplicationLifetime hostApplicationLifetime)
//        {
//            _logger = logger;
//            _simpleWorkerHelper = simpleWorkerHelper;
//            _hostApplicationLifetime = hostApplicationLifetime;
//            _hostApplicationLifetime.ApplicationStopping.Register(async () => {

//                Log("ApplicationStopping Begin");
//                if (!_simpleWorkerHelper.ShouldWorking)
//                {
//                    Log("ApplicationStopping End");
//                    return;
//                }

//                _shouldStop = true;
//                Log("DoTheClean Begin");
//                await DoTheClean();
//                Log("DoTheClean End");
//            });
//        }

//        public override async Task StartAsync(CancellationToken cancellationToken)
//        {
//            Log("StartAsync");
//            await base.StartAsync(cancellationToken);
//        }

//        public override async Task StopAsync(CancellationToken cancellationToken)
//        {
//            Log("StopAsync Begin");
//            _shouldStop = true;
//            while (!_cleanCompleted)
//            {
//                await Task.Delay(TimeSpan.FromSeconds(1));
//            }
//            Log("StopAsync End");
//            await base.StopAsync(cancellationToken);
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            try
//            {
//                Log("ExecuteAsync Begin");
//                // 这里实现实际的业务逻辑
//                while (!stoppingToken.IsCancellationRequested)
//                {
//                    if (_shouldStop)
//                    {
//                        Log("should stop working");
//                        break;
//                    }

//                    if (!_simpleWorkerHelper.ShouldWorking)
//                    {
//                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
//                        continue;
//                    }

//                    await DoTheWork(stoppingToken);
//                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
//                }
//            }
//            finally
//            {
//                Log("ExecuteAsync DoTheClean");
//                await DoTheClean();
//                Log("ExecuteAsync End");
//            }
//        }

//        private async Task DoTheWork(CancellationToken cancellationToken)
//        {
//            _simpleWorkerHelper.AddExecutedCount();
//            Log("Working... => " + _simpleWorkerHelper.ExecutedCount);
//            await Task.CompletedTask;
//        }


//        private bool doCleaning = false;
//        private async Task DoTheClean()
//        {
//            if (doCleaning == true)
//            {
//                return;
//            }

//            doCleaning = true;
//            //clean
//            for (int i = 0; i < 3; i++)
//            {
//                Log("DoTheClean => " + (3 - i));
//                await Task.Delay(TimeSpan.FromSeconds(1));
//            }
//            _cleanCompleted = true;
//            Log("DoTheClean => completed!");
//            await Task.CompletedTask;
//        }

//        private void Log(string msg, LogLevel logLevel = LogLevel.Information)
//        {
//            var threadInfo = $"";
//            _logger.Log(logLevel, ">>> " + msg + threadInfo);
//        }
//    }

//    public class SimpleWorkerHelper
//    {
//        private long executedCount;

//        public long ExecutedCount { get => executedCount; private set => executedCount = value; }
//        public bool ShouldWorking { get; set; }

//        public void AddExecutedCount()
//        {
//            executedCount = Interlocked.Increment(ref executedCount);
//        }
//    }
//}
