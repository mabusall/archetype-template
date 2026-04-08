//namespace Tasheer.Core.Api.Features.Orders;

//class ChangeStatusOrderCommandHandler(IFlixHubDbUnitOfWork uow)
//   : ICommandHandler<ChangeStatusOrderCommand, ChangeStatusOrderResult>
//{
//    public async Task<ChangeStatusOrderResult> Handle(ChangeStatusOrderCommand command, CancellationToken cancellationToken)
//    {
//        var Order = await uow
//            .OrdersRepository
//            .AsQueryable(true)
//            .FirstOrDefaultAsync(user => user.Uuid == command.UserId, cancellationToken);

//        Order!.IsActive = command.IsActive;

//        // update the record
//        uow.OrdersRepository.Update(Order);

//        // commit db changes
//        await uow.SaveChangesAsync(cancellationToken);

//        // clear change tracker
//        uow.ClearChangeTracker();

//        return new ChangeStatusOrderResult(true);
//    }
//}