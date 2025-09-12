namespace DemoProductApi.Domain.Entities;

public class BundleItem
{
    public Guid Id { get; set; }                 // New PK (simplifies uniqueness)
    public Guid BundleId { get; set; }
    public Guid ChildProductItemId { get; set; }
    public decimal Quantity { get; set; }
    public Bundle Bundle { get; set; } = default!;
    public ProductItem? ChildProductItem { get; set; }
}