using System;

namespace DemoProductApi.Application.Models;

public class BundleDto
{
    public Guid BundleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<BundleItemDto> Items { get; set; } = new();
    public List<BundlePricingRuleDto> PricingRules { get; set; } = new();
}

public class BundleItemDto
{
    public Guid BundleId { get; set; }
    public Guid ChildId { get; set; }
    public decimal Quantity { get; set; }
}

public class BundlePricingRuleDto
{
    public Guid BundlePricingRuleId { get; set; }
    public Guid BundleId { get; set; }
    public int RuleType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? PercentOff { get; set; }
    public int ApplyTo { get; set; }
}