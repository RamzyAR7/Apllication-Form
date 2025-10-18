using FluentValidation;
using MediatR;
using Application_Form.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Behaviors
{
    public class ValidationBehaviorForSimpleResult<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehaviorForSimpleResult<TRequest, TResponse>> _logger;

        public ValidationBehaviorForSimpleResult(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviorForSimpleResult<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
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
                return (TResponse)(object)Result.Failure(messages);
            }

            return await next();
        }
    }
}
