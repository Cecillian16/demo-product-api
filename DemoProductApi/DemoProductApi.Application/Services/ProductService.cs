using DemoProductApi.Domain;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class ProductService(IProductRepository repo) : IProductService
{
    public async Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        return entity is null ? null : ProductMapper.ToDto(entity);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateRequest request, CancellationToken ct = default)
    {
        // Build a DTO without Ids; mapper will generate them.
        var dto = new ProductDto
        {
            ProductId = Guid.Empty,
            Name = request.Name,
            SkuPrefix = request.SkuPrefix,
            Description = request.Description,
            Status = request.Status,
            VariantOptions = request.VariantOptions.Select(vo => new VariantOptionDto
            {
                VariantOptionId = Guid.Empty,
                Name = vo.Name,
                Values = vo.Values.Select(v => new VariantOptionValueDto
                {
                    VariantOptionValueId = Guid.Empty,
                    Value = v.Value,
                    Code = v.Code
                }).ToList()
            }).ToList()
        };

        var entity = ProductMapper.ToEntity(dto);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return ProductMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != dto.ProductId)
            return false;

        var entity = await repo.GetByIdAsync(id, includeDetails: true, ct);
        if (entity is null)
            return false;

        entity.Name = dto.Name;
        entity.SkuPrefix = dto.SkuPrefix;
        entity.Description = dto.Description;
        entity.Status = (Status)dto.Status;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        repo.Update(entity);
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

    // Keeping domain entities for list; change to DTO projection if needed.
    public async Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(
        int page, int size, string? search, Status? status, CancellationToken ct = default)
    {
        var total = await repo.CountAsync(search, status, ct);
        var items = await repo.GetPagedAsync(page, size, search, status, includeDetails: false, ct);
        return (items, total);
    }
}