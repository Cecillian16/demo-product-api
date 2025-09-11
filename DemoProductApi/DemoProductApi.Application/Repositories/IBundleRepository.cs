using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface IBundleRepository
{
    Task<Bundle?> GetByIdAsync(Guid bundleId, bool includeDetails = false, CancellationToken ct = default);
    Task AddAsync(Bundle bundle, CancellationToken ct = default);
    void Update(Bundle bundle);
    void Remove(Bundle bundle);
    Task SaveChangesAsync(CancellationToken ct = default);
}