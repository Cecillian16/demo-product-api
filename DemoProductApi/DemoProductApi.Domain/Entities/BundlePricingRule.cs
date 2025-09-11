namespace DemoProductApi.Domain.Entities;

public class BundlePricingRule
{
    public Guid BundlePricingRuleId { get; set; }
    public Guid BundleId { get; set; }
    public BundlePricingRuleType RuleType { get; set; }
    public decimal? Amount { get; set; }       // for Fixed
    public decimal? PercentOff { get; set; }   // for PercentOff
    public ApplyToScope ApplyTo { get; set; } = ApplyToScope.All;

    public Bundle Bundle { get; set; } = null!;
}