namespace DemoProductApi.Application.Models.Requests;

public class ProductItemCreateRequest
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = default!;
    public int Status { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public List<ProductItemVariantValueCreateRequest> VariantValues { get; set; } = new();
}

public class ProductItemVariantValueCreateRequest
{
    public Guid VariantOptionId { get; set; }
    public Guid VariantOptionValueId { get; set; }
}