namespace BaseApp.Infra.Bootstrap.Version;

using Microsoft.AspNetCore.Builder;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public static class VersionStartup
{
    public static IServiceCollection AddVersion(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            })
            .AddApiVersioning(config => { config.ReportApiVersions = true; });
    }

    public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/status", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        current_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        status = report.Status.ToString(),
                        machine = Environment.MachineName
                    });

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }
        });
        return app;
    }
}
