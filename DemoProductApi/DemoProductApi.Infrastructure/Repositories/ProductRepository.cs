using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid productId, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<Product> q = db.Products;

        if (includeDetails)
        {
            q = q
                .Include(p => p.VariantOptions)
                    .ThenInclude(vo => vo.Values)
                .Include(p => p.ProductItems);
        }

        return await q.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }

    public async Task<IReadOnlyList<Product>> GetPagedAsync(
        int page, int pageSize,
        string? search = null,
        Status? status = null,
        bool includeDetails = false,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Product> q = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{term}%"));
        }

        if (status is not null)
            q = q.Where(p => p.Status == status);

        if (includeDetails)
        {
            q = q
                .Include(p => p.VariantOptions)
                    .ThenInclude(vo => vo.Values)
                .Include(p => p.ProductItems);
        }

        q = q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking();

        return await q.ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, Status? status = null, CancellationToken ct = default)
    {
        IQueryable<Product> q = db.Products;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{term}%"));
        }

        if (status is not null)
            q = q.Where(p => p.Status == status);

        return await q.CountAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
        => await db.Products.AddAsync(product, ct);

    public void Update(Product product)
        => db.Products.Update(product);

    public void Remove(Product product)
        => db.Products.Remove(product);

    public async Task<Product?> GetWithInventoryAndPricesAsync(
        Guid productId,
        string? currency = null,
        DateOnly? onDate = null,
        CancellationToken ct = default)
    {
        // Base query with details
        var q = db.Products
            .Include(p => p.ProductItems)
                .ThenInclude(i => i.Inventories)
            .Include(p => p.ProductItems)
            .AsQueryable();

        var product = await q.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
        if (product is null)
            return null;

        // Load prices (Product + each ProductItem)
        // Separate query to avoid cartesian explosion
        var entityIds = new List<Guid> { product.ProductId };
        entityIds.AddRange(product.ProductItems.Select(i => i.ProductItemId));

        var today = onDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var pricesQuery = db.Prices
            .Where(pr =>
                entityIds.Contains(pr.EntityId) &&
                (currency == null || pr.Currency == currency) &&
                pr.ValidFrom <= today &&
                (pr.ValidTo == null || pr.ValidTo >= today));

        // Materialize to ensure it's fetched now (attach to context if needed elsewhere)
        _ = await pricesQuery.ToListAsync(ct);

        return product;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}