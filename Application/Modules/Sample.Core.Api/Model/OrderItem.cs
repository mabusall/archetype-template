namespace Sample.Core.Api.Model;

public record OrderItemDto : AuditableDto
{
    public int OrderId { get; set; }

    public string? Name { get; set; }
}