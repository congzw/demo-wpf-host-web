using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Demo.Web.Bootstrap
{
    public static class SwaggerSetup
    {
        public static IServiceCollection AddTheSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("默认分组", new OpenApiInfo { Title = "默认分组", Version = "v1.0.0" });

                //// 为 Swagger JSON and UI设置xml文档注释路径
                ////var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //var basePath = AppDomain.CurrentDomain.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, "NbSites.Web.xml");
                //c.IncludeXmlComments(xmlPath);
            });
            return services;
        }

        public static IApplicationBuilder UseTheSwagger(this IApplicationBuilder app)
        {
            ////hack virtual app path bugs!!!
            var docPrefix = "/ApiDoc";
            var virtualPath = ""; //todo: read from config
            if (!string.IsNullOrWhiteSpace(virtualPath))
            {
                docPrefix = docPrefix + virtualPath.TrimStart('/').TrimEnd('/');
            }
            // Sets a custom route for the Swagger JSON endpoint(s). Must include the {documentName} parameter
            app.UseSwagger(c => c.RouteTemplate = docPrefix + "/{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/SampleApi/swagger/v1/swagger.json", "Sample API");
                //c.RoutePrefix = "SampleApi/swagger";
                c.SwaggerEndpoint(docPrefix + "/默认分组/swagger.json", "默认分组");
                c.RoutePrefix = docPrefix.TrimStart('/');
            });

            return app;
        }
    }
}
