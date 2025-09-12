using DemoProductApi.Domain;

namespace DemoProductApi.Application.Models;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SkuPrefix { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<VariantOptionDto> VariantOptions { get; set; } = new();
}

public class VariantOptionDto
{
    public Guid VariantOptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<VariantOptionValueDto> Values { get; set; } = new();
}

public class VariantOptionValueDto
{
    public Guid VariantOptionValueId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? Code { get; set; }
}