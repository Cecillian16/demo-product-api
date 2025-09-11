namespace DemoProductApi.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Type { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}