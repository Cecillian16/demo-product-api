namespace DemoProductApi.Application.Models;

public class ProductItemDto
{
    public Guid ProductItemId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Status { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public List<ProductItemVariantValueDto> VariantValues { get; set; } = new();
}

public class ProductItemVariantValueDto
{
    public Guid VariantOptionId { get; set; }
    public Guid VariantOptionValueId { get; set; }
}