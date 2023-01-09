using Demo.Web.Api;
using Demo.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoWorkerWeb.Api
{
    [Route("Api/SimpleWorker/GetDesc")]
    public class SimpleWorkerApi : BaseApi
    {
        private readonly ILogger<SimpleWorkerApi> _logger;
        private readonly SimpleWorkerHelper simpleWorkerHelper;

        public SimpleWorkerApi(ILogger<SimpleWorkerApi> logger, SimpleWorkerHelper simpleWorkerHelper)
        {
            _logger = logger;
            this.simpleWorkerHelper = simpleWorkerHelper;
        }

        [HttpGet]
        public string GetDesc()
        {
            _logger.LogInformation("from " + this.GetType().Name);
            return this.GetType().Name;
        }

        [HttpGet]
        [Route("Api/SimpleWorker/Status")]
        public string WorkerStatus()
        {
            if (simpleWorkerHelper.ShouldWorking)
            {
                return "working start at " + simpleWorkerHelper.ExecutedCount;
            }
            else
            {
                return "working pause at " + simpleWorkerHelper.ExecutedCount;
            }
        }

        [HttpPost]
        [Route("Api/SimpleWorker/Start")]
        public string WorkerStart()
        {
            simpleWorkerHelper.ShouldWorking = true;
            return "start working at " + simpleWorkerHelper.ExecutedCount; ;
        }

        [HttpPost]
        [Route("Api/SimpleWorker/Stop")]
        public string WorkerStop()
        {
            simpleWorkerHelper.ShouldWorking = false;
            return "stop working at " + simpleWorkerHelper.ExecutedCount;
        }

    }
}