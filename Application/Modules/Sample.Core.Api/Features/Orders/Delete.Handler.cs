namespace Sample.Core.Api.Features.Orders;

internal class DeleteOrderCommandHandler(ISampleDbUnitOfWork uow)
    : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        // get the order info
        Order? order = await uow
            .OrdersRepository
            .AsQueryable(true)
            .FirstOrDefaultAsync(o => o.Id == command.OrderId, cancellationToken);

        // delete the order from db
        uow.OrdersRepository.Delete(order!);

        // commit db changes
        await uow.SaveChangesAsync(cancellationToken);

        // clear change tracker to prevent memory leaks
        uow.ClearChangeTracker();

        // return result
        return new DeleteOrderResult(true);
    }
}