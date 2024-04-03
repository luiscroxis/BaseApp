namespace BaseApp.Infra.Bootstrap.Swagger;

using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerStartup
{
    public static IServiceCollection AddSwaggerApi<TOperationFilter, TDocumentFilter>(this IServiceCollection services, string applicationName)
        where TOperationFilter : IOperationFilter
        where TDocumentFilter : IDocumentFilter
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.DescribeAllParametersInCamelCase();
            using var serviceProvider = services.BuildServiceProvider();
            var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                {
                    Title = $"{applicationName} {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated ? $"{applicationName} - DEPRECATED" : $"{applicationName}"
                });
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location) ?? string.Empty, $"{a.Name}.xml"))
                .Where(File.Exists).ToArray();

            Array.ForEach(xmlDocs, (d) => { options.IncludeXmlComments(d); });

            options.OperationFilter<TOperationFilter>();
            options.DocumentFilter<TDocumentFilter>();
        });

        return services;
    }
}
