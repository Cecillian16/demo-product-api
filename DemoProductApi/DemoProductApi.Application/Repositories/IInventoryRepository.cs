using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default);
    Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken ct = default);
    Task<Inventory?> GetByProductItemAndLocationAsync(Guid productItemId, Guid locationId, bool asTracking = false, CancellationToken ct = default);
    Task<IReadOnlyList<Inventory>> GetByProductItemAsync(Guid productItemId, CancellationToken ct = default);

    Task AddAsync(Inventory inventory, CancellationToken ct = default);
    void Update(Inventory inventory);
    void Remove(Inventory inventory);
    Task SaveChangesAsync(CancellationToken ct = default);
}