namespace Sample.Core.Api.Features.Orders;

internal class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator(ISampleDbUnitOfWork uow)
    {
        RuleFor(r => r.OrderId)
            .GreaterThan(0)
            .WithMessage(ErrorMessageResources.NotEmpty)

            // check if the sign in is mobile user
            .DependentRules(() =>
            {
                RuleFor(r => r.OrderId)
                    .MustAsync(async (id, cancellation) =>
                    {
                        Order? order = await uow.OrdersRepository.GetByIdAsync(id, cancellation);
                        return order is not null;
                    })
                    .WithMessage(ErrorMessageResources.Orders_OrderNotFound);
            });
    }
}