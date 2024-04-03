namespace BaseApp.Infra.Bootstrap.Database;

using System.Diagnostics.CodeAnalysis;
using Domain.Repository.Orm.Abstract.Contexts;
using Domain.Repository.Orm.Abstract.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository.Orm.Repositories;
using DbContext = Repository.Orm.Contexts.DbContext;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IDbContext, DbContext>(
            opt =>
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseNpgsql(configuration["ConnectionStrings:Connection"])
                    .EnableSensitiveDataLogging()
        );

        //services.AddDbContext<DbContextUser>(
        //  opt =>
        //      opt.UseNpgsql(configuration["ConnectionStrings:Connection"])
        //          .EnableSensitiveDataLogging()
        //);

        //services.AddIdentity<User, Roles>(options =>
        //{
        //    options.Lockout.AllowedForNewUsers = true;
        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        //    options.Lockout.MaxFailedAccessAttempts = 50;

        //    options.Password.RequireDigit = false;
        //    options.Password.RequiredLength = 6;
        //   options.Password.RequiredUniqueChars = 1;
        //    options.Password.RequireLowercase = false;
        //    options.Password.RequireNonAlphanumeric = false;
        //    options.Password.RequireUppercase = false;

        //    options.SignIn.RequireConfirmedEmail = false;
        //    options.SignIn.RequireConfirmedPhoneNumber = false;
        //    options.User.RequireUniqueEmail = true;

        //}).AddEntityFrameworkStores<DbContextUser>()
        //.AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "",
                ValidAudience = ""
            };
        });

        services.AddAuthorization();
        services.AddMemoryCache();
        services.AddJwksManager().PersistKeysInMemory().UseJwtValidation();

        services.AddEndpointsApiExplorer();

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        return services;
    }
}
