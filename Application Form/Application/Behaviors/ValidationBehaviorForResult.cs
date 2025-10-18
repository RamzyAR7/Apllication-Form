using FluentValidation;
using MediatR;
using Application_Form.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Behaviors
{
    public class ValidationBehaviorForResult<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehaviorForResult<TRequest, TResponse>> _logger;

        public ValidationBehaviorForResult(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviorForResult<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(
            TRequest request,
            RequestHandlerDelegate<Result<TResponse>> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var messages = string.Join("; ", failures.Select(f => f.ErrorMessage));
                _logger.LogWarning("Validation failed for {RequestType}: {Messages}", typeof(TRequest).Name, messages);
                return Result<TResponse>.Failure(messages);
            }

            return await next();
        }
    }
}
