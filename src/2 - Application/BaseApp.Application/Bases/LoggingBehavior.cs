namespace BaseApp.Application.Bases;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Domain.Service.Abstract.Dtos.Bases.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage]
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ResponseDto<TResponse>> where TRequest : IRequest<ResponseDto<TResponse>>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<ResponseDto<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ResponseDto<TResponse>> next, CancellationToken cancellationToken)
    {
        var timer = new Stopwatch();
        timer.Start();
        var handlerName = nameof(TRequest);
        _logger.LogTrace("Commando iniciado :{0}\nRequest: {1}", handlerName, JsonConvert.SerializeObject(request));

        try
        {
            var response = await next().ConfigureAwait(false);
            _logger.LogTrace("Comando executado: {0}\nStatusCode: {1}", handlerName, response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex.Message, ex);
            throw;
        }
        finally
        {
            timer.Stop();
            var seconds = timer.Elapsed.TotalSeconds;
            _logger.LogTrace("Comando finalizado: {0}\nTempo: {2}s", handlerName, seconds);
        }
    }
}
