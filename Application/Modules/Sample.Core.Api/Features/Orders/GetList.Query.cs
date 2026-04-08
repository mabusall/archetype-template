namespace Sample.Core.Api.Features.Orders;

public class GetListOrderQuery
    : PagingBase, IQuery<PaginatedList<OrderDto>>
{
    public int? OrderId { get; set; }
    public bool? IncludeItems { get; set; }
}
