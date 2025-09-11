namespace DemoProductApi.Domain.Entities;

public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductItemId { get; set; }
    public Guid LocationId { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int InTransit { get; set; }
    public int ReorderPoint { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ProductItem ProductItem { get; set; } = null!;
    public Location Location { get; set; } = null!;

    public int Available => OnHand - Reserved;
}