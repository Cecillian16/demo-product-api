using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models;

public static class ProductItemMapper
{
    public static ProductItemDto ToDto(ProductItem entity) => new()
    {
        ProductItemId = entity.ProductItemId,
        ProductId = entity.ProductId,
        Sku = entity.Sku,
        Status = (int)entity.Status,
        Weight = entity.Weight,
        Volume = entity.Volume,
        VariantValues = entity.VariantValues?.Select(vv => new ProductItemVariantValueDto
        {
            VariantOptionId = vv.VariantOptionId,
            VariantOptionValueId = vv.VariantOptionValueId
        }).ToList() ?? new()
    };

    // For create: ignore any incoming ProductItemId, always generate new and set timestamps
    public static ProductItem ToNewEntity(ProductItemDto dto)
    {
        var now = DateTimeOffset.UtcNow;
        var newId = Guid.NewGuid();

        var entity = new ProductItem
        {
            ProductItemId = newId,
            ProductId = dto.ProductId,
            Sku = dto.Sku,
            Status = (Status)dto.Status,
            Weight = dto.Weight,
            Volume = dto.Volume,
            CreatedAt = now,
            UpdatedAt = now,
            VariantValues = dto.VariantValues?.Select(vv => new ProductItemVariantValue
            {
                ProductItemId = newId,
                VariantOptionId = vv.VariantOptionId,
                VariantOptionValueId = vv.VariantOptionValueId
            }).ToList() ?? new()
        };
        return entity;
    }

    // Legacy/general (trusting provided IDs) - prefer not to use on create
    public static ProductItem ToEntity(ProductItemDto dto)
    {
        var entity = new ProductItem
        {
            ProductItemId = dto.ProductItemId,
            ProductId = dto.ProductId,
            Sku = dto.Sku,
            Status = (Status)dto.Status,
            Weight = dto.Weight,
            Volume = dto.Volume,
            VariantValues = dto.VariantValues?.Select(vv => new ProductItemVariantValue
            {
                ProductItemId = dto.ProductItemId,
                VariantOptionId = vv.VariantOptionId,
                VariantOptionValueId = vv.VariantOptionValueId
            }).ToList() ?? new()
        };
        return entity;
    }
}