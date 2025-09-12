using DemoProductApi.Domain;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class ProductService(IGenericRepository<Product> repo) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await repo.GetAllAsync(ct);
        return entities.Select(ProductMapper.ToDto).ToList();
    }

    public async Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? null : ProductMapper.ToDto(entity);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateRequest request, CancellationToken ct = default)
    {
        var entity = ProductMapper.ToEntity(request, Guid.Empty);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return ProductMapper.ToDto(entity);
    }

    // Replace strategy: delete existing product (and cascaded VariantOptions/Values) then recreate from DTO payload.
    public async Task<bool> UpdateAsync(Guid id, ProductCreateRequest request, CancellationToken ct = default)
    {
        if (id == Guid.Empty) return false;

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        var originalCreatedAt = existing.CreatedAt;
        var replacement = ProductMapper.ToEntity(request, id);
        replacement.CreatedAt = originalCreatedAt; // preserve original CreatedAt

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

    // Delete product and all related VariantOptions / VariantOptionValues.
    // Assumes EF Core cascade delete is configured:
    // Product -> VariantOption: OnDelete Cascade
    // VariantOption -> VariantOptionValue: OnDelete Cascade
    // If not, configure in AppDbContext.OnModelCreating or manually load & remove children.
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty) return false;

        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null) return false;

        repo.Remove(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}