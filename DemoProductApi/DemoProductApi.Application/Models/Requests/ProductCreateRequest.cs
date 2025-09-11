namespace DemoProductApi.Application.Models.Requests;

public class ProductCreateRequest
{
    public string Name { get; set; } = default!;
    public string SkuPrefix { get; set; } = default!;
    public string? Description { get; set; }
    public int Status { get; set; }
    public List<VariantOptionCreateRequest> VariantOptions { get; set; } = new();
}

public class VariantOptionCreateRequest
{
    public string Name { get; set; } = default!;
    public List<VariantOptionValueCreateRequest> Values { get; set; } = new();
}

public class VariantOptionValueCreateRequest
{
    public string Value { get; set; } = default!;
    public string? Code { get; set; }
}