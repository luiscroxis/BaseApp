namespace BaseApp.API.Filters;

using Domain.Service.Abstract.Dtos.Bases.Responses;
using Extensions;
using Infra.CrossCuting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidatorFilter : IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(ResponseDto<None>.Fail(context.ModelState.GetErrors()));
            return;
        }

        await next().ConfigureAwait(false);
    }
}
