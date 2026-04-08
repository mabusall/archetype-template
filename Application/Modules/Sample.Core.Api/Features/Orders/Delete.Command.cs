namespace Sample.Core.Api.Features.Orders;

public record DeleteOrderCommand(int OrderId) : ICommand<DeleteOrderResult>;

public record DeleteOrderResult(bool IsSuccess);