namespace DemoProductApi.Domain.Entities;

public class Price
{
    public Guid Id { get; set; }
    public PriceEntityType EntityType { get; set; } // Product or Item
    public Guid EntityId { get; set; }              // Points to ProductId or ProductItemId
    public string Currency { get; set; } = "USD";
    public decimal ListPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}