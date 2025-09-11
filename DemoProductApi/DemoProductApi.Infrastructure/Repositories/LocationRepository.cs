using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class LocationRepository(AppDbContext db) : ILocationRepository
{
    public async Task<Location?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default)
    {
        var q = db.Locations.AsQueryable();
        if (!asTracking) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default) =>
        await db.Locations.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Location location, CancellationToken ct = default) =>
        await db.Locations.AddAsync(location, ct);

    public void Update(Location location) =>
        db.Locations.Update(location);

    public void Remove(Location location) =>
        db.Locations.Remove(location);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}