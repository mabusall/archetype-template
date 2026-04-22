namespace Sample.Core.Api.Features.Orders;

internal class CreateOrderCommandHandler(ISampleDbUnitOfWork uow)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = command.Adapt<Order>();

        // create random price for the order between 5000 and 25000 with decimal points
        order.Price = (decimal)(new Random().Next(5000, 25000) + new Random().NextDouble());

        // calculate VAT and total price
        order.Vat = order.Price * 15 / 100;
        order.TotalPrice = order.Price + order.Vat;
        order.Items = [.. command.Orders.Select(o => new OrderItem { Name = o })];

        // add user to database
        uow.OrdersRepository.Insert(order);

        // commit the changes
        await uow.SaveChangesAsync(cancellationToken);

        // clear change tracker
        uow.ClearChangeTracker();

        return new CreateOrderResult(order.Uuid, order.Price);
    }
}