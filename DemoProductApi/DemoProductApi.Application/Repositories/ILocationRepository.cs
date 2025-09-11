using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default);
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Location location, CancellationToken ct = default);
    void Update(Location location);
    void Remove(Location location);
    Task SaveChangesAsync(CancellationToken ct = default);
}