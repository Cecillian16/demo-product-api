namespace DemoProductApi.Domain.Entities;

public class ProductItem
{
    public Guid ProductItemId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = "";
    public string? Barcode { get; set; }
    public Status Status { get; set; } = Status.Active;
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Product Product { get; set; } = default!;
    public ICollection<ProductItemVariantValue> VariantValues { get; set; } = new List<ProductItemVariantValue>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}