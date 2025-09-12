using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models;

public static class ProductItemMapper
{
    public static ProductItem ToEntity(ProductItemCreateRequest dto, Guid id)
    {
        var now = DateTimeOffset.UtcNow;
        var productId = id != Guid.Empty ? id : Guid.NewGuid();

        var entity = new ProductItem
        {
            ProductItemId = productId,
            ProductId = dto.ProductId,
            Sku = dto.Sku,
            Status = (Status)dto.Status,
            Weight = dto.Weight,
            Volume = dto.Volume,
            CreatedAt = now,
            UpdatedAt = now,
            VariantValues = dto.VariantValues?.Select(vv => new ProductItemVariantValue
            {
                ProductItemId = productId,
                VariantOptionId = vv.VariantOptionId,
                VariantOptionValueId = vv.VariantOptionValueId
            }).ToList() ?? new()
        };
        return entity;
    }

    public static ProductItemDto ToDto(ProductItem entitiy)
    {
        var now = DateTimeOffset.UtcNow;

        var productItem = new ProductItemDto
        {
            ProductItemId = entitiy.ProductItemId,
            ProductId = entitiy.ProductId,
            Sku = entitiy.Sku,
            Status = (int)entitiy.Status,
            Weight = entitiy.Weight,
            Volume = entitiy.Volume,
            VariantValues = entitiy.VariantValues?.Select(vv => new ProductItemVariantValueDto
            {
                VariantOptionId = vv.VariantOptionId,
                VariantOptionValueId = vv.VariantOptionValueId
            }).ToList() ?? new()
        };

        return productItem;
    }
}