using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class ProductItemRepository(AppDbContext db) : IGenericRepository<ProductItem>
{
    public async Task<IReadOnlyList<ProductItem>> GetAllAsync(CancellationToken ct = default)
    {
        IQueryable<ProductItem> q = db.ProductItems;

        q = q
            .Include(i => i.Product)
            .Include(i => i.VariantValues)
                .ThenInclude(v => v.VariantOptionValue)
            .Include(i => i.VariantValues)
                .ThenInclude(v => v.VariantOption);

        return await q
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<ProductItem?> GetByIdAsync(Guid productItemId, CancellationToken ct = default)
    {
        IQueryable<ProductItem> q = db.ProductItems;

        q = q
            .Include(i => i.Product)
            .Include(i => i.VariantValues)
                .ThenInclude(v => v.VariantOptionValue)
            .Include(i => i.VariantValues)
                .ThenInclude(v => v.VariantOption);

        return await q.FirstOrDefaultAsync(i => i.ProductItemId == productItemId, ct);
    }

    public async Task AddAsync(ProductItem item, CancellationToken ct = default)
        => await db.ProductItems.AddAsync(item, ct);

    public void Update(ProductItem item)
        => db.ProductItems.Update(item);

    public void Remove(ProductItem item)
        => db.ProductItems.Remove(item);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    => db.Database.BeginTransactionAsync(ct);
}