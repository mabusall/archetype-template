namespace Sample.Core.Api.Features.Orders;

internal class GetListOrderCommandHandler(ISampleDbUnitOfWork uow)
    : IQueryHandler<GetListOrderQuery, PaginatedList<OrderDto>>
{
    public async Task<PaginatedList<OrderDto>> Handle(GetListOrderQuery query, CancellationToken cancellationToken)
    {
        var querable = uow
            .OrdersRepository
            .AsQueryable(false);

        if (query.OrderId is not null)
            querable = querable.Where(w => w.Id == query.OrderId);

        if (query.IncludeItems == true)
            querable = querable.Include(i => i.Items);

        // execute query on db
        return await uow
            .OrdersRepository
            .GetPaginatedListAsync(querable,
                                   query.PageNumber,
                                   query.PageSize,
                                   query.SortBy!,
                                   cancellationToken);
    }
}