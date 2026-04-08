namespace Sample.Core.Api.Repository;

#region [ interfaces ]

interface IOrdersRepository : IGenericRepository<Order, int, OrderDto> { }

interface IOrderItemsRepository : IGenericRepository<OrderItem, int, OrderItemDto> { }

#endregion

#region [ implementation ]

class OrdersRepository(SampleDbContext context)
    : GenericRepository<Order, int, OrderDto>(context), IOrdersRepository
{ }

class OrderItemsRepository(SampleDbContext context)
    : GenericRepository<OrderItem, int, OrderItemDto>(context), IOrderItemsRepository
{ }

#endregion