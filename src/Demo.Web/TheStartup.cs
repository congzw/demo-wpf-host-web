using Demo.Web.Bootstrap;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Demo.Workers;
using Common.Workers;

namespace Demo.Web
{
    public class TheStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<SimpleWorker>();
            services.AddSingleton<SimpleWorkerHelper>();
            services.AddHostedService<HostEventService>();

            services.AddControllers();
            services.AddTheSwaggerGen();

            services.AddSingleton(TheWebHelper.Instance);
        }

        public void Configure(IApplicationBuilder app)
        {
            TheWebHelper.Instance.RootServiceProvider = app.ApplicationServices;

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseTheSwagger();
            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
            });
        }
    }
}
