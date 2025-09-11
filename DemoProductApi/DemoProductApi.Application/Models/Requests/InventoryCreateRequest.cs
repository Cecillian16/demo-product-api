namespace DemoProductApi.Application.Models.Requests;

public class InventoryCreateRequest
{
    public Guid ProductItemId { get; set; }
    public Guid LocationId { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int InTransit { get; set; }
    public int ReorderPoint { get; set; }
}