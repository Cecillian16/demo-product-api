namespace DemoProductApi.Domain.Entities;

public class VariantOption
{
    public Guid VariantOptionId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = ""; // e.g., Size, Color

    public Product Product { get; set; } = default!;
    public ICollection<VariantOptionValue> Values { get; set; } = new List<VariantOptionValue>();
}