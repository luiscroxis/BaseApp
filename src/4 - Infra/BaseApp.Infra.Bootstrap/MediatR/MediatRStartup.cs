namespace BaseApp.Infra.Bootstrap.MediatR;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Application.Bases;
using global::MediatR;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class MediatRStartup
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(FailRequestBehaviorWithResponseHandler<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailRequestBehaviorWithResponseHandler<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddMediatR(assembly!);

        return services;
    }
}
