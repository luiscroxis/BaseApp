namespace BaseApp.Application.Bases;

using System.Diagnostics.CodeAnalysis;
using Domain.Service.Abstract.Dtos;
using Domain.Service.Abstract.Dtos.Bases.Responses;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

[ExcludeFromCodeCoverage]
public class FailRequestBehaviorWithResponseHandler<TRequest, TResponse> : IPipelineBehavior<TRequest, ResponseDto<TResponse>>
    where TRequest : IRequest<ResponseDto<TResponse>>
{
    private readonly IEnumerable<IValidator> _validators;

    public FailRequestBehaviorWithResponseHandler(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    private static Task<ResponseDto<TResponse>> Errors(IEnumerable<ValidationFailure> failures)
        => Task.FromResult(ResponseDto<TResponse>.Fail(failures.Select(x =>
            ErrorResponse.CreateError(x.ErrorMessage)
                .WithDeveloperMessage(x.ToString()))
        ));

    public Task<ResponseDto<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ResponseDto<TResponse>> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        return failures.Any()
            ? Errors(failures)
            : next();
    }
}
