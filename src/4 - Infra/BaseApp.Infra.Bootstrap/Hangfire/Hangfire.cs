namespace BaseApp.Infra.Bootstrap.Hangfire;

using System.Diagnostics.CodeAnalysis;
using global::Hangfire;
using global::Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class Hangfire
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration["ConnectionStrings:Connection"]));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        return services;
    }
}
