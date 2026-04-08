namespace Sample.Core.Api.Features.Orders;

public record CreateOrderCommand
(
    // common fields for all users type
    OrderType OrderType,
    IList<string> Orders
) : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id, decimal Price);