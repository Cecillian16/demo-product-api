using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class ProductItemService(IGenericRepository<ProductItem> repo) : IProductItemService
{
    public async Task<ProductItemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? null : ProductItemMapper.ToDto(entity);
    }

    public async Task<IReadOnlyList<ProductItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await repo.GetAllAsync(ct);
        return entities.Select(ProductItemMapper.ToDto).ToList();
    }

    public async Task<ProductItemDto> CreateAsync(ProductItemCreateRequest request, CancellationToken ct = default)
    {
        var entity = ProductItemMapper.ToEntity(request, Guid.Empty);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return ProductItemMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, ProductItemCreateRequest request, CancellationToken ct = default)
    {
        if (id == Guid.Empty) return false;

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        var replacement = ProductItemMapper.ToEntity(request, id);

        await repo.Update(existing, replacement, ct);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        repo.Remove(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}