namespace BaseApp.Infra.Bootstrap.Validation;

using System.Globalization;
using Application.Bases;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

public static class ValidationStartup
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidation(
            s =>
            {
                s.LocalizationEnabled = true;
                s.DisableDataAnnotationsValidation = true;
                s.ValidatorOptions.LanguageManager.Culture = new CultureInfo("pt-BR");
                s.RegisterValidatorsFromAssemblyContaining(typeof(RequestValidator<>));
            });
        return services;
    }
}
