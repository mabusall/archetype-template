namespace Archetype.Infrastructure.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        ValidationContext<TRequest> validationContext = new(request);

        FluentValidation.Results.ValidationResult[] validationResults =
            await Task.WhenAll(validators.Select(s => s.ValidateAsync(validationContext, cancellationToken)));

        List<FluentValidation.Results.ValidationFailure> failures =
            validationResults
            .Where(w => w.Errors.Count != 0)
            .SelectMany(vr => vr.Errors)
            .ToList();

        if (failures.Count != 0) throw new FluentValidation.ValidationException(failures);

        return await next(cancellationToken);
    }
}