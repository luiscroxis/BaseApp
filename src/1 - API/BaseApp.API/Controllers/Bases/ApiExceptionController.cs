namespace BaseApp.API.Controllers.Bases;

using System.Net;
using Domain.Service.Abstract.Dtos;
using Domain.Service.Abstract.Dtos.Bases.Responses;
using Infra.CrossCuting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ApiExceptionController : ApiResultController
{
    [Route("/error"), HttpGet("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GetError([FromServices] IWebHostEnvironment env)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var status = (HttpStatusCode)HttpContext.Response.StatusCode;
        var response = ResponseDto.Fail(ErrorResponse.CreateError(Constants.DefaultFail)
                .WithDeveloperMessage(context?.Error.Message)
                .WithStackTrace(context?.Error.StackTrace)
                .WithException(context?.Error.ToString())
                .WithErrorCode(HttpContext.Response.StatusCode.ToString()),
            (HttpStatusCode)HttpContext.Response.StatusCode
        );

        return CreateResult(response, status);
    }
}
