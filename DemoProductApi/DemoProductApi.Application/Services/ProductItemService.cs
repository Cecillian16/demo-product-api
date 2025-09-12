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

        // Transaction ensures atomicity of delete + recreate
        await using var tx = await repo.BeginTransactionAsync(ct);
        try
        {
            repo.Remove(existing);
            await repo.SaveChangesAsync(ct);          // executes DELETE + cascades

            await repo.AddAsync(replacement, ct);     // stage INSERT + children
            await repo.SaveChangesAsync(ct);          // executes INSERTS

            await tx.CommitAsync(ct);
            return true;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
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