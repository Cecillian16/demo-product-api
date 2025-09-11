using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class InventoryRepository(AppDbContext db) : IInventoryRepository
{
    public async Task<Inventory?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default)
    {
        var q = db.Inventories.AsQueryable();
        if (!asTracking) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken ct = default) =>
        await db.Inventories.AsNoTracking().ToListAsync(ct);

    public async Task<Inventory?> GetByProductItemAndLocationAsync(Guid productItemId, Guid locationId, bool asTracking = false, CancellationToken ct = default)
    {
        var q = db.Inventories.AsQueryable();
        if (!asTracking) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(i => i.ProductItemId == productItemId && i.LocationId == locationId, ct);
    }

    public async Task<IReadOnlyList<Inventory>> GetByProductItemAsync(Guid productItemId, CancellationToken ct = default) =>
        await db.Inventories
            .AsNoTracking()
            .Where(i => i.ProductItemId == productItemId)
            .ToListAsync(ct);

    public async Task AddAsync(Inventory inventory, CancellationToken ct = default)
        => await db.Inventories.AddAsync(inventory, ct);

    public void Update(Inventory inventory)
        => db.Inventories.Update(inventory);

    public void Remove(Inventory inventory)
        => db.Inventories.Remove(inventory);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}