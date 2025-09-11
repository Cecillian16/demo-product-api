namespace DemoProductApi.Domain.Entities;

public class Bundle
{
    public Guid BundleId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Status Status { get; set; } = Status.Active;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<BundleItem> Items { get; set; } = new List<BundleItem>();
    public ICollection<BundlePricingRule> PricingRules { get; set; } = new List<BundlePricingRule>();
}