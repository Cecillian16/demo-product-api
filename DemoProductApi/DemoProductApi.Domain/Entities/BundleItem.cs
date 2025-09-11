namespace DemoProductApi.Domain.Entities;

public class BundleItem
{
    public Guid BundleId { get; set; }
    public Guid ChildId { get; set; }
    public decimal Quantity { get; set; }

    public Bundle Bundle { get; set; } = default!;
    public ProductItem? ChildItem { get; set; }
    public Bundle? ChildBundle { get; set; }
}