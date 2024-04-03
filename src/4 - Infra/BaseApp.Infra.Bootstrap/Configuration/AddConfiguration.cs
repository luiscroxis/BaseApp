namespace BaseApp.Infra.Bootstrap.Configuration;

using System.Globalization;
using System.Text.Json.Serialization;
using CrossCuting;
using Domain.Service.Abstract.Dtos;
using Domain.Service.Abstract.Dtos.Bases.Responses;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Version;

public static class AddConfiguration
{
    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, params Type[] filters)
    {
        services.AddFluentValidationRulesToSwagger();
        services.AddControllers(options =>
        {
            if (filters?.Length > 0)
                foreach (var filter in filters)
                    options.Filters.Add(filter);
        }).AddConfigureApiBehavior()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var state = context.ModelState
                        .Where(x => x.Value?.ValidationState == ModelValidationState.Invalid)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => ErrorResponse.CreateError(x.ErrorMessage)
                            .WithDeveloperMessage(x.Exception?.Message)
                            .WithStackTrace(x.Exception?.StackTrace)
                            .WithException(x.Exception?.ToString()));
                    return new BadRequestObjectResult(ResponseDto<None>.Fail(state));
                };
            });

        return services;
    }

    //public static IApplicationBuilder AtualizarDados(this WebApplication app)
    //{
    //    var dashboardOptions =
    //       new DashboardOptions
    //       {
    //           Authorization = new[] { new DashboardNoAuthorizationFilter() }
    //       };
    //    //Hangfire
    //   app.UseHangfireDashboard("/hangfire", dashboardOptions);
        //======== Configuração dos JOB´s ==================================
        //Envio de e-mail - [Executa todos os dias as 06:00]
    //    RecurringJob.AddOrUpdate<IEmailServicesService>(
   //         "Envio de e-mail",
   //         x => x.EnvioEmailServiceAsync(default),
   //         "5 * * * *");

   //     RecurringJob.AddOrUpdate<IGifttyDBService>(
   //         "Atualiza Produtos Giftty",
   //         x => x.AtualizarProdutosAsync(default),
   //        "0 5 * * *");

   //     return app;
   // }

    //public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
   // {
   //     public bool Authorize(DashboardContext dashboardContext)
   //     {
   //         return true;
   //     }
   // }

    public static IApplicationBuilder UseDefaultConfigure(this WebApplication app, IWebHostEnvironment env, string applicationName)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            var provider = app.Services.GetService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                foreach (var description in provider!.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            });
        }

        if (env.IsProduction())
        {
            app.UseSwagger();
            var provider = app.Services.GetService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                foreach (var description in provider!.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            });
        }

        var defaultDateCulture = "pt-BR";
        var ci = new CultureInfo(defaultDateCulture);
        ci.NumberFormat.NumberDecimalSeparator = ",";
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

        // Configure the Localization middleware
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(ci),
            SupportedCultures = new List<CultureInfo>
            {
                ci,
            },
            SupportedUICultures = new List<CultureInfo>
            {
                ci,
            }
        });

        app.UseExceptionHandler("/error");
        app.UseHealthCheck();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        return app;
    }

    private static IMvcBuilder AddConfigureApiBehavior(this IMvcBuilder builder)
        =>
            builder.AddJsonOptions(
                options => { options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never; });
}
