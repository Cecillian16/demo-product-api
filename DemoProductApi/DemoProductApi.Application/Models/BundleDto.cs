namespace DemoProductApi.Application.Models;

public class BundleDto
{
    public Guid BundleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<BundleItemDto> Items { get; set; } = new();
}

public class BundleItemDto
{
    public Guid? ChildProductItemId { get; set; }
    public decimal Quantity { get; set; }
}