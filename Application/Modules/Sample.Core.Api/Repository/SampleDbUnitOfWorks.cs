namespace Sample.Core.Api.Repository;

interface ISampleDbUnitOfWork : IUnitOfWork
{
    IOrdersRepository OrdersRepository { get; }

    IOrderItemsRepository OrderItemsRepository { get; }
}

class SampleDbUnitOfWork(SampleDbContext context) : UnitOfWork(context), ISampleDbUnitOfWork
{
    public IOrdersRepository OrdersRepository => new OrdersRepository(context);

    public IOrderItemsRepository OrderItemsRepository => new OrderItemsRepository(context);
}