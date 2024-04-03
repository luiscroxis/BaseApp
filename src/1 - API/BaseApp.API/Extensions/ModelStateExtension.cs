namespace BaseApp.API.Extensions;

using Domain.Service.Abstract.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public static class ModelStateExtension
{
    public static List<ErrorResponse> GetErrors(this ModelStateDictionary state)
        => state.SelectMany(x => x.Value!.Errors).Select(x => ErrorResponse.CreateError(x.ErrorMessage)
            .WithDeveloperMessage(x.Exception?.Message)
            .WithStackTrace(x.Exception?.StackTrace)
            .WithException(x.Exception?.ToString())).ToList();
}
