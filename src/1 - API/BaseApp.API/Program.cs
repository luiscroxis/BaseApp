using BaseApp.API.Filters;
using BaseApp.Infra.Bootstrap.AutoMapper;
using BaseApp.Infra.Bootstrap.Configuration;
using BaseApp.Infra.Bootstrap.Database;
using BaseApp.Infra.Bootstrap.Hangfire;
using BaseApp.Infra.Bootstrap.MediatR;
using BaseApp.Infra.Bootstrap.Service;
using BaseApp.Infra.Bootstrap.Swagger;
using BaseApp.Infra.Bootstrap.Validation;
using BaseApp.Infra.Bootstrap.Version;

var applicationName = "BaseApp";
var builder = WebApplication.CreateBuilder(args);


builder
    .Services
    //.AddCustomAutoMapper()
    .AddCustomMediatR()
    .AddServices()
    .AddRepositories(builder.Configuration)
    .AddHangfire(builder.Configuration)
    .AddVersion()
    .AddValidation()
    .AddSwaggerApi<RemoveQueryApiVersionParamOperationFilter, RemoveDefaultApiVersionRouteDocumentFilter>(applicationName)
    .AddCustomConfiguration(typeof(ValidatorFilter))
    .AddCors(options =>
    {
        options.AddPolicy(name: "CorsPolicy",
            policy =>
            {
                policy.WithOrigins("*")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseDefaultConfigure(app.Environment, applicationName);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
