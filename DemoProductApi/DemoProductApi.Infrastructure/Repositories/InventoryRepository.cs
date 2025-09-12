using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class InventoryRepository(AppDbContext db) : IGenericRepository<Inventory>
{
    public async Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken ct = default) =>
        await db.Inventories.AsNoTracking().ToListAsync(ct);

    public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Inventories.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Inventory inventory, CancellationToken ct = default)
        => await db.Inventories.AddAsync(inventory, ct);

    public void Update(Inventory inventory)
        => db.Inventories.Update(inventory);

    public void Remove(Inventory inventory)
        => db.Inventories.Remove(inventory);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    => db.Database.BeginTransactionAsync(ct);

}