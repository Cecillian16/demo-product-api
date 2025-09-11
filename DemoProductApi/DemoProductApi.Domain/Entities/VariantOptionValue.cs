namespace DemoProductApi.Domain.Entities;

public class VariantOptionValue
{
    public Guid VariantOptionValueId { get; set; }
    public Guid VariantOptionId { get; set; }
    public string Value { get; set; } = ""; // e.g., L, Red
    public string? Code { get; set; }       // e.g., "L", "RED"

    public VariantOption VariantOption { get; set; } = default!;
    public ICollection<ProductItemVariantValue> ProductItemVariantValues { get; set; } = new List<ProductItemVariantValue>();
}