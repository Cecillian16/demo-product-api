using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models;

public static class ProductMapper
{
    public static Product ToEntity(ProductDto dto)
    {
        var now = DateTimeOffset.UtcNow;
        var productId = Guid.NewGuid();

        var product = new Product
        {
            ProductId = productId,
            Name = dto.Name,
            SkuPrefix = dto.SkuPrefix,
            Description = dto.Description,
            Status = (Status)dto.Status,
            CreatedAt = now,
            UpdatedAt = now,
            VariantOptions = dto.VariantOptions.Select(vo =>
            {
                var variantOptionId = Guid.NewGuid();
                return new VariantOption
                {
                    VariantOptionId = variantOptionId,
                    Name = vo.Name,
                    ProductId = productId,
                    Values = vo.Values.Select(vv => new VariantOptionValue
                    {
                        VariantOptionValueId = Guid.NewGuid(),
                        Value = vv.Value,
                        Code = vv.Code,
                        VariantOptionId = variantOptionId
                    }).ToList()
                };
            }).ToList()
        };

        return product;
    }

    public static ProductDto ToDto(Product entity)
    {
        return new ProductDto
        {
            ProductId = entity.ProductId,
            Name = entity.Name,
            SkuPrefix = entity.SkuPrefix,
            Description = entity.Description,
            Status = (int)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            VariantOptions = entity.VariantOptions?.Select(vo => new VariantOptionDto
            {
                VariantOptionId = vo.VariantOptionId,
                Name = vo.Name,
                Values = vo.Values?.Select(vv => new VariantOptionValueDto
                {
                    VariantOptionValueId = vv.VariantOptionValueId,
                    Value = vv.Value,
                    Code = vv.Code
                }).ToList() ?? new List<VariantOptionValueDto>()
            }).ToList() ?? new List<VariantOptionDto>()
        };
    }
}