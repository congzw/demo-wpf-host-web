using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Web.Api
{
    [Route("~/Api/Test/[action]")]
    public class TestApi : BaseApi
    {
        private readonly IServiceProvider _serviceProvider;

        public TestApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        [HttpGet]
        public DateTime GetServerNow()
        {
            return DateTime.Now;
        }
        
        [HttpGet]
        public async Task<DateTime> GetSlow()
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            return DateTime.Now;
        }

        [HttpGet]
        public List<string> GetServices()
        {
            var list = new List<string>();

            var test1 = _serviceProvider.GetService<IFooService1>();
            list.Add($"IFooService1: {test1?.GetDesc()}");

            var test2 = _serviceProvider.GetService<IFooService2>();
            list.Add($"IFooService2: {test2?.GetDesc()}");

            return list;
        }

        [HttpGet]
        public string SendMessageToWindow([FromQuery]string msg)
        {
            var svc = _serviceProvider.GetService<IWinControlService>();
            if (svc == null)
            {
                return "Fail";
            }
            return svc.SendMessage(msg);
        }

        [HttpPost]
        [Route("Api/Demo/App/ShutDown")]
        public string AppShutDown([FromServices] IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
                lifetime.StopApplication();
            }
            return "OK";
        }

        [HttpGet]
        [Route("Api/Demo/App/GetLifetime")]
        public string GetLifetime([FromServices] IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
                return lifetime.GetType().FullName;
            }
        }
    }

    #region demo

    public interface IFooService1
    {
        string GetDesc();
    }

    public interface IFooService2
    {
        string GetDesc();
    }

    public class FooService1 : IFooService1
    {
        public string GetDesc()
        {
            return $"{this.GetType().FullName} - {this.GetHashCode()}";
        }
    }

    public interface IWinControlService
    {
        string SendMessage(string msg);
    }

    #endregion
}