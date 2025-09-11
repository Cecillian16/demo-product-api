using DemoProductApi.Domain.Entities;
using DemoProductApi.Domain;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class BundleService(IBundleRepository repo) : IBundleService
{
    public async Task<BundleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        return entity is null ? null : BundleMapper.ToDto(entity);
    }

    public async Task<BundleDto> CreateAsync(BundleCreateRequest request, CancellationToken ct = default)
    {
        var dto = new BundleDto
        {
            BundleId = Guid.Empty, // ignored anyway
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            Items = request.Items,
            PricingRules = request.PricingRules
        };
        var entity = BundleMapper.ToNewEntity(dto);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return BundleMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, BundleDto dto, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != dto.BundleId)
            return false;

        var bundle = await repo.GetByIdAsync(id, includeDetails: true, ct);
        if (bundle is null)
            return false;

        // Scalars
        bundle.Name = dto.Name;
        bundle.Description = dto.Description;
        bundle.Status = (Status)dto.Status;
        bundle.UpdatedAt = DateTimeOffset.UtcNow;

        // ---- Merge BundleItems (by ChildId) ----
        var incomingItems = dto.Items ?? new List<BundleItemDto>();
        var incomingChildIds = incomingItems.Select(i => i.ChildId).ToHashSet();

        // Remove missing
        foreach (var existing in bundle.Items.Where(i => !incomingChildIds.Contains(i.ChildId)).ToList())
            bundle.Items.Remove(existing);

        // Add / update
        foreach (var itemDto in incomingItems)
        {
            var existing = bundle.Items.FirstOrDefault(i => i.ChildId == itemDto.ChildId);
            if (existing is not null)
            {
                existing.Quantity = itemDto.Quantity;
            }
            else
            {
                bundle.Items.Add(new BundleItem
                {
                    BundleId = bundle.BundleId,
                    ChildId = itemDto.ChildId,
                    Quantity = itemDto.Quantity
                });
            }
        }

        // ---- Merge PricingRules (by BundlePricingRuleId) ----
        var incomingRules = dto.PricingRules ?? new List<BundlePricingRuleDto>();

        // Assign new IDs for newly added
        foreach (var r in incomingRules.Where(r => r.BundlePricingRuleId == Guid.Empty))
            r.BundlePricingRuleId = Guid.NewGuid();

        var incomingRuleIds = incomingRules.Select(r => r.BundlePricingRuleId).ToHashSet();

        // Remove missing
        foreach (var existingRule in bundle.PricingRules.Where(r => !incomingRuleIds.Contains(r.BundlePricingRuleId)).ToList())
            bundle.PricingRules.Remove(existingRule);

        // Add / update
        foreach (var ruleDto in incomingRules)
        {
            var existing = bundle.PricingRules.FirstOrDefault(r => r.BundlePricingRuleId == ruleDto.BundlePricingRuleId);
            if (existing is not null)
            {
                existing.RuleType = (BundlePricingRuleType)ruleDto.RuleType;
                existing.Amount = ruleDto.Amount;
                existing.PercentOff = ruleDto.PercentOff;
                existing.ApplyTo = (ApplyToScope)ruleDto.ApplyTo;
            }
            else
            {
                bundle.PricingRules.Add(new BundlePricingRule
                {
                    BundlePricingRuleId = ruleDto.BundlePricingRuleId,
                    BundleId = bundle.BundleId,
                    RuleType = (BundlePricingRuleType)ruleDto.RuleType,
                    Amount = ruleDto.Amount,
                    PercentOff = ruleDto.PercentOff,
                    ApplyTo = (ApplyToScope)ruleDto.ApplyTo
                });
            }
        }

        repo.Update(bundle);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        if (entity is null)
            return false;

        repo.Remove(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}