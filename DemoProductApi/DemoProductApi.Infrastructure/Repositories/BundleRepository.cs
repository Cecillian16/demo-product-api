using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class BundleRepository(AppDbContext db) : IGenericRepository<Bundle>
{
    public async Task<IReadOnlyList<Bundle>> GetAllAsync(CancellationToken ct = default)
    {
        IQueryable<Bundle> q = db.Bundles;

        q = q
            .Include(b => b.Items);

        return await q.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Bundle?> GetByIdAsync(Guid bundleId, CancellationToken ct = default)
    {
        IQueryable<Bundle> q = db.Bundles;

        q = q
            .Include(b => b.Items);

        return await q.FirstOrDefaultAsync(b => b.BundleId == bundleId, ct);
    }

    public async Task AddAsync(Bundle bundle, CancellationToken ct = default)
        => await db.Bundles.AddAsync(bundle, ct);

    public void Update(Bundle bundle)
        => db.Bundles.Update(bundle);

    public void Remove(Bundle bundle)
        => db.Bundles.Remove(bundle);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => db.Database.BeginTransactionAsync(ct);
}