using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models;

public static class BundleMapper
{
    public static Bundle ToNewEntity(BundleDto dto)
    {
        var now = DateTimeOffset.UtcNow;
        var bundleId = dto.BundleId != Guid.Empty ? dto.BundleId : Guid.NewGuid();

        var entity = new Bundle
        {
            BundleId = bundleId,
            Name = dto.Name,
            Description = dto.Description,
            Status = (Status)dto.Status,
            CreatedAt = now,
            UpdatedAt = now,
            Items = dto.Items?.Select(it => new BundleItem
            {
                BundleId = bundleId,
                ChildId = it.ChildId,
                Quantity = it.Quantity
            }).ToList() ?? new List<BundleItem>(),
            PricingRules = dto.PricingRules?.Select(r =>
            {
                var ruleId = r.BundlePricingRuleId != Guid.Empty ? r.BundlePricingRuleId : Guid.NewGuid();
                return new BundlePricingRule
                {
                    BundlePricingRuleId = ruleId,
                    BundleId = bundleId,
                    RuleType = (BundlePricingRuleType)r.RuleType,
                    Amount = r.Amount,
                    PercentOff = r.PercentOff,
                    ApplyTo = (ApplyToScope)r.ApplyTo
                };
            }).ToList() ?? new List<BundlePricingRule>()
        };

        return entity;
    }

    public static BundleDto ToDto(Bundle entity)
    {
        return new BundleDto
        {
            BundleId = entity.BundleId,
            Name = entity.Name,
            Description = entity.Description,
            Status = (int)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Items = entity.Items?.Select(i => new BundleItemDto
            {
                ChildId = i.ChildId,
                Quantity = i.Quantity
            }).ToList() ?? new List<BundleItemDto>(),
            PricingRules = entity.PricingRules?.Select(r => new BundlePricingRuleDto
            {
                BundlePricingRuleId = r.BundlePricingRuleId,
                RuleType = (int)r.RuleType,
                Amount = r.Amount,
                PercentOff = r.PercentOff,
                ApplyTo = (int)r.ApplyTo
            }).ToList() ?? new List<BundlePricingRuleDto>()
        };
    }
}