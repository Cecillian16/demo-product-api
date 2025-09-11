using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class ProductItemService(IProductItemRepository repo) : IProductItemService
{
    public async Task<ProductItemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        return entity is null ? null : ProductItemMapper.ToDto(entity);
    }

    public async Task<ProductItemDto> CreateAsync(ProductItemCreateRequest request, CancellationToken ct = default)
    {
        var dto = new ProductItemDto
        {
            ProductItemId = Guid.Empty,
            ProductId = request.ProductId,
            Sku = request.Sku,
            Status = request.Status,
            Weight = request.Weight,
            Volume = request.Volume,
            VariantValues = request.VariantValues.Select(v => new ProductItemVariantValueDto
            {
                VariantOptionId = v.VariantOptionId,
                VariantOptionValueId = v.VariantOptionValueId
            }).ToList()
        };

        var entity = ProductItemMapper.ToNewEntity(dto);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return ProductItemMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, ProductItemDto dto, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != dto.ProductItemId)
            return false;

        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        if (entity is null)
            return false;

        // Scalars
        entity.Sku = dto.Sku;
        entity.Status = (Status)dto.Status;
        entity.Weight = dto.Weight;
        entity.Volume = dto.Volume;
        entity.ProductId = dto.ProductId;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        var incoming = dto.VariantValues ?? new List<ProductItemVariantValueDto>();

        // Enforce uniqueness of VariantOptionId
        if (incoming.GroupBy(v => v.VariantOptionId).Any(g => g.Count() > 1))
            return false;

        var existingByOption = entity.VariantValues.ToDictionary(v => v.VariantOptionId, v => v);

        // Remove missing
        foreach (var existing in entity.VariantValues
                     .Where(ev => !incoming.Any(iv => iv.VariantOptionId == ev.VariantOptionId))
                     .ToList())
        {
            entity.VariantValues.Remove(existing); // Cascade delete will handle removal
        }

        // Add / update
        foreach (var iv in incoming)
        {
            if (existingByOption.TryGetValue(iv.VariantOptionId, out var existing))
            {
                if (existing.VariantOptionValueId != iv.VariantOptionValueId)
                    existing.VariantOptionValueId = iv.VariantOptionValueId;
            }
            else
            {
                entity.VariantValues.Add(new ProductItemVariantValue
                {
                    ProductItemId = entity.ProductItemId,
                    VariantOptionId = iv.VariantOptionId,
                    VariantOptionValueId = iv.VariantOptionValueId
                });
            }
        }

        repo.Update(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        if (entity is null)
            return false;

        // Variant values will cascade on delete
        repo.Remove(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}