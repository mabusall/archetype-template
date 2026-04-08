namespace Sample.Core.Api.Repository;

internal class OrderConfiguration : BaseEntityConfiguration<Order, int>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        // Call base configuration for audit properties and base indexes (Uuid, Created)
        base.Configure(builder);

        // Configure Order-specific properties
        builder.Property(p => p.Price)
            .HasComment("The price of the order before tax.");

        builder.Property(p => p.Vat)
            .HasComment("The value-added tax (VAT) applied to the order.");

        builder.Property(p => p.TotalPrice)
            .HasComment("The total price of the order including VAT.");

        builder.Property(p => p.OrderType)
            .HasComment("The type of the order (Pending, Completed, Cancelled, etc.).");

        // Configure entity-specific indexes
        // OrderType - Frequently filtered (e.g., WHERE OrderType = Pending)
        builder.HasIndex(p => p.OrderType)
            .HasDatabaseName("IX_Order_OrderType");
    }
}

internal class OrderItemConfiguration : BaseEntityConfiguration<OrderItem, int>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Call base configuration for audit properties and base indexes (Uuid, Created)
        base.Configure(builder);

        // Configure OrderItem-specific properties
        builder.Property(p => p.OrderId)
            .HasComment("Foreign key to the associated order.");

        builder.Property(p => p.Name)
            .HasComment("The name or description of the order item.");

        // Configure entity-specific indexes
        // OrderId - Foreign key (used in JOINs and filtering)
        // SQL Server does NOT automatically create indexes on foreign keys!
        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("IX_OrderItem_OrderId");
    }
}