namespace Sample.Core.Api.Entities;

[Table(nameof(OrderItem))]
class OrderItem : AuditableEntity<int>
{
    [Required, ForeignKey(nameof(Order)), Column(Order = 1)]
    public int OrderId { get; set; }

    [Required, MaxLength(50), Column(TypeName = "nvarchar", Order = 2)]
    public required string Name { get; set; }
}