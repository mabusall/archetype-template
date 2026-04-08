namespace Sample.Core.Api.Model;

public record OrderDto : AuditableDto
{
    public decimal Price { get; set; }

    public decimal Vat { get; set; }

    public decimal TotalPrice { get; set; }

    public OrderType OrderType { get; set; }

    public List<OrderItemDto> Items { get; set; } = [];
}