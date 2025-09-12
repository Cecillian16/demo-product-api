namespace DemoProductApi.Application.Models.Requests;

public class BundleCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public List<BundleItemCreateRequest> Items { get; set; } = new();
}

public class BundleItemCreateRequest
{
    public Guid? ChildProductItemId { get; set; }
    public decimal Quantity { get; set; }
}