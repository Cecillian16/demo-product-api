namespace DemoProductApi.Domain.Entities;

public class Bundle
{
    public Guid BundleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<BundleItem> Items { get; set; } = new List<BundleItem>();
}