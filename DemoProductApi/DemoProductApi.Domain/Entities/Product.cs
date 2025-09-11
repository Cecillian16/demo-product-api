namespace DemoProductApi.Domain.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = "";
    public string SkuPrefix { get; set; } = "";
    public Status Status { get; set; } = Status.Active;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<VariantOption> VariantOptions { get; set; } = new List<VariantOption>();
    public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}