using DemoProductApi.Application.Models;

namespace DemoProductApi.Application.Models.Requests;

public class BundleCreateRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int Status { get; set; }
    public List<BundleItemDto> Items { get; set; } = new();
    public List<BundlePricingRuleDto> PricingRules { get; set; } = new();
}