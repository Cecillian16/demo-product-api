using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class ProductItemRepository(AppDbContext db) : IProductItemRepository
{
    public async Task<ProductItem?> GetByIdAsync(Guid productItemId, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<ProductItem> q = db.ProductItems;

        if (includeDetails)
        {
            q = q
                .Include(i => i.Product)
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOptionValue)
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOption)
                .Include(i => i.Inventories);
        }

        return await q.FirstOrDefaultAsync(i => i.ProductItemId == productItemId, ct);
    }

    public async Task<ProductItem?> GetBySkuAsync(string sku, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<ProductItem> q = db.ProductItems;

        if (includeDetails)
        {
            q = q
                .Include(i => i.Product)
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOptionValue)
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOption)
                .Include(i => i.Inventories);
        }

        return await q.FirstOrDefaultAsync(i => i.Sku == sku, ct);
    }

    public async Task<IReadOnlyList<ProductItem>> GetByProductAsync(Guid productId, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<ProductItem> q = db.ProductItems.Where(i => i.ProductId == productId);

        if (includeDetails)
        {
            q = q
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOptionValue)
                .Include(i => i.VariantValues)
                    .ThenInclude(v => v.VariantOption)
                .Include(i => i.Inventories);
        }

        return await q.AsNoTracking().ToListAsync(ct);
    }

    public async Task AddAsync(ProductItem item, CancellationToken ct = default)
        => await db.ProductItems.AddAsync(item, ct);

    public void Update(ProductItem item)
        => db.ProductItems.Update(item);

    public void Remove(ProductItem item)
        => db.ProductItems.Remove(item);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}