using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private static readonly Type _typeForShouldValidateAsyncInterface = typeof(IShouldValidateAsync);

        private readonly IEnumerable<IValidator<TRequest>> _allValidators;

        private readonly IEnumerable<IValidator<TRequest>> _asynchronousValidators;

        private readonly IEnumerable<IValidator<TRequest>> _synchronousValidators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _allValidators = validators;

            _asynchronousValidators =
                _allValidators
                .Select(validator => (Type: validator.GetType(), Validator: validator))
                .Where(tuple => _typeForShouldValidateAsyncInterface.IsAssignableFrom(tuple.Type)
                    && !tuple.Type.IsInterface
                    && !tuple.Type.IsAbstract)
                .Select(tuple => tuple.Validator);

            _synchronousValidators =
                _allValidators.Except(_asynchronousValidators);
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);

            var synchronousValidatorFailures = _synchronousValidators
                .Select(validator => validator.Validate(context));

            var asynchronousValidatorFailures = await Task.WhenAll
                (
                    _asynchronousValidators
                    .Select(async validator => await validator.ValidateAsync(context))
                );

            var validationFailures = synchronousValidatorFailures
                .Concat(asynchronousValidatorFailures)
                .SelectMany(result => result.Errors)
                .Where(validationFailure => validationFailure != null)
                .ToList();

            if (validationFailures.Count() != 0)
            {
                throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
