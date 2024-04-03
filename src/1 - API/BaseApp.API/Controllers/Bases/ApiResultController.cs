namespace BaseApp.API.Controllers.Bases;

using System.Collections;
using System.Net;
using Domain.Service.Abstract.Dtos.Bases.Responses;
using Infra.CrossCuting;
using Microsoft.AspNetCore.Mvc;

public class ApiResultController : ControllerBase
{
    /// <summary>
    /// Criar uma resposta da chamada API
    /// </summary>
    /// <param name="code">StatusCode de resposta</param>
    /// <returns>Retorna um resultado customizado</returns>
    protected IActionResult CreateResult(HttpStatusCode code) => CreateResult<None>(null, code);

    /// <summary>
    /// Criar uma resposta da chamada API
    /// </summary>
    /// <param name="dto">Data da resposta</param>
    /// <param name="code">StatusCode de resposta</param>
    /// <returns>Retorna um resultado customizado</returns>
    protected IActionResult CreateResult<TResponse>(ResponseDto<TResponse>? dto, HttpStatusCode code = HttpStatusCode.OK)
    {
        var status = (int)code;
        var statusDto = (int)(dto?.StatusCode ?? 0);

        if (status >= 500)
            return StatusCode(status, dto);

        if ((int)(dto?.StatusCode ?? 0) >= 500)
            return StatusCode(statusDto, dto);

        var msg = dto?.GetType().GetProperty("Msg")?.GetValue(dto, null);

        if (code is HttpStatusCode.NoContent || dto?.StatusCode is HttpStatusCode.NoContent && msg is not { })
            return NoContent();

        if (code is HttpStatusCode.NoContent || dto?.StatusCode is HttpStatusCode.NoContent && msg is { })
            return Ok(dto);

        if (code is HttpStatusCode.BadRequest || dto?.StatusCode is HttpStatusCode.BadRequest)
            return BadRequest(dto);

        var data = dto?.GetType().GetProperty("Data")?.GetValue(dto, null);

        if (data is IList { Count: <= 0 } || code is HttpStatusCode.NotFound || dto?.StatusCode is HttpStatusCode.NotFound)
            return NotFound(dto);

        if (code is HttpStatusCode.OK && dto?.StatusCode is HttpStatusCode.OK && data is { })
            return Ok(dto);

        return NotFound(null);
    }
}
