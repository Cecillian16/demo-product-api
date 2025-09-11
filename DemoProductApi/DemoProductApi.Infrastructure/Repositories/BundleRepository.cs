using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class BundleRepository(AppDbContext db) : IBundleRepository
{
    public async Task<Bundle?> GetByIdAsync(Guid bundleId, bool includeDetails = false, CancellationToken ct = default)
    {
        IQueryable<Bundle> q = db.Bundles;

        if (includeDetails)
        {
            q = q
                .Include(b => b.Items)
                .Include(b => b.PricingRules);
        }

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
}