namespace Sample.Core.Api.Features.Orders;

internal class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(r => r.OrderType)
            .IsInEnum()
            .WithMessage(ErrorMessageResources.NotEmpty);

        RuleFor(r => r.Orders)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);

        RuleForEach(r => r.Orders)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}