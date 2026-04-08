namespace Sample.Core.Api.Entities;

[Table(nameof(Order))]
class Order : AuditableEntity<int>
{
    [Required, Column(TypeName = "decimal(18,2)", Order = 1)]
    public decimal Price { get; set; }

    [Required, Column(TypeName = "decimal(18,2)", Order = 2)]
    public decimal Vat { get; set; }

    [Required, Column(TypeName = "decimal(18,2)", Order = 3)]
    public decimal TotalPrice { get; set; }

    [Required, Column(TypeName = "smallint", Order = 4)]
    public OrderType OrderType { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}