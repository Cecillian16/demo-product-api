using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class PriceRepository(AppDbContext db) : IPriceRepository
{
    public async Task<IReadOnlyList<Price>> GetAllAsync(CancellationToken ct = default) =>
        await db.Prices.AsNoTracking().ToListAsync(ct);

    public async Task<Price?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default)
    {
        var q = db.Prices.AsQueryable();
        if (!asTracking) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Price>> GetActiveAsync(
        PriceEntityType entityType,
        Guid entityId,
        DateOnly onDate,
        string? currency = null,
        CancellationToken ct = default)
    {
        var q = db.Prices
            .AsNoTracking()
            .Where(p =>
                p.EntityType == entityType &&
                p.EntityId == entityId &&
                p.ValidFrom <= onDate &&
                (p.ValidTo == null || p.ValidTo >= onDate));

        if (!string.IsNullOrWhiteSpace(currency))
            q = q.Where(p => p.Currency == currency);

        return await q
            .OrderByDescending(p => p.ValidFrom)
            .ThenBy(p => p.SalePrice == null) // sale prices first (false < true)
            .ThenBy(p => p.ListPrice)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Price>> GetHistoryAsync(
        PriceEntityType entityType,
        Guid entityId,
        int? top = null,
        CancellationToken ct = default)
    {
        var q = db.Prices
            .AsNoTracking()
            .Where(p => p.EntityType == entityType && p.EntityId == entityId)
            .OrderByDescending(p => p.ValidFrom)
            .ThenByDescending(p => p.ValidTo);

        if (top is > 0)
            q = (IOrderedQueryable<Price>)q.Take(top.Value);

        return await q.ToListAsync(ct);
    }

    public async Task AddAsync(Price price, CancellationToken ct = default) =>
        await db.Prices.AddAsync(price, ct);

    public void Update(Price price) =>
        db.Prices.Update(price);

    public void Remove(Price price) =>
        db.Prices.Remove(price);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}