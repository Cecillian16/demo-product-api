using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext db) : IGenericRepository<Product>
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        IQueryable<Product> q = db.Products;

        q = q
            .Include(p => p.VariantOptions)
                .ThenInclude(vo => vo.Values)
            .Include(p => p.ProductItems);

        return await q.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct = default)
    {
        IQueryable<Product> q = db.Products;

        q = q
            .Include(p => p.VariantOptions)
                .ThenInclude(vo => vo.Values)
            .Include(p => p.ProductItems);

        return await q.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
        => await db.Products.AddAsync(product, ct);

    public async Task Update(Product existing, Product replacement, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        try
        {
            db.Products.Remove(existing);
            await db.SaveChangesAsync(ct);

            await db.Products.AddAsync(replacement, ct);
            await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public void Remove(Product product)
        => db.Products.Remove(product);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => db.Database.BeginTransactionAsync(ct);
}