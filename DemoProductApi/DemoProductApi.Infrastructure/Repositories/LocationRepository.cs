using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class LocationRepository(AppDbContext db) : IGenericRepository<Location>
{
    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default) =>
        await db.Locations.AsNoTracking().ToListAsync(ct);

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Locations.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Location location, CancellationToken ct = default) =>
        await db.Locations.AddAsync(location, ct);

    public void Update(Location location) =>
        db.Locations.Update(location);

    public void Remove(Location location) =>
        db.Locations.Remove(location);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    => db.Database.BeginTransactionAsync(ct);
}