using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
    }
}