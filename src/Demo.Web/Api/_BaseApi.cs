using Microsoft.AspNetCore.Mvc;

namespace Demo.Web.Api
{
    public interface IApi { }

    [IgnoreAntiforgeryToken]
    [ApiController]
    public abstract class BaseApi : IApi
    {
        [ApiExplorerSettings(GroupName = "HideGroup")]
        [HttpGet]
        public virtual string GetStatus()
        {
            return this.GetType().FullName;
        }
    }
}
