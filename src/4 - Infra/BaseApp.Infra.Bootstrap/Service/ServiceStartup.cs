namespace BaseApp.Infra.Bootstrap.Service;

using System.Diagnostics.CodeAnalysis;
using Domain.Repository.Orm.Abstract.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Repository.Orm.UnitOfWork;

[ExcludeFromCodeCoverage]
public static class ServiceStartup
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
