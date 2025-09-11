namespace DemoProductApi.Domain.Entities;

public class ProductItemVariantValue
{
    // Composite key: ProductItemId + VariantOptionId
    public Guid ProductItemId { get; set; }
    public Guid VariantOptionId { get; set; }
    public Guid VariantOptionValueId { get; set; }

    public ProductItem ProductItem { get; set; } = default!;
    public VariantOption VariantOption { get; set; } = default!;
    public VariantOptionValue VariantOptionValue { get; set; } = default!;
}