using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Application_Form.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
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

            var failures = new List<ValidationFailure>();
            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(context, cancellationToken);
                if (validationResult?.Errors != null)
                {
                    failures.AddRange(validationResult.Errors.Where(f => f != null));
                }
            }

            if (failures.Any())
            {
                var messages = string.Join("\n ", failures.Select(f => f.ErrorMessage));
                _logger.LogWarning("Validation failed for {RequestType}: {Messages}", typeof(TRequest).Name, messages);

                // Check if TResponse is Result<T>
                var responseType = typeof(TResponse);
                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var t = responseType.GetGenericArguments()[0];
                    var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });
                    return (TResponse)failureMethod.Invoke(null, new object[] { messages });
                }
                else
                {
                    // For other types, throw ValidationException
                    throw new ValidationException(messages);
                }
            }

            return await next();
        }
    }
}
