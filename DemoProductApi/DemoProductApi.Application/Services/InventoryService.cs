using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class InventoryService(IGenericRepository<Inventory> repo) : IInventoryService
{
    public Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken ct = default) =>
        repo.GetAllAsync(ct);

    public Task<Inventory?> GetAsync(Guid id, CancellationToken ct = default) =>
        repo.GetByIdAsync(id, ct);

    public async Task<Inventory> CreateAsync(InventoryCreateRequest request, CancellationToken ct = default)
    {
        var entity = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductItemId = request.ProductItemId,
            LocationId = request.LocationId,
            OnHand = request.OnHand,
            Reserved = request.Reserved,
            InTransit = request.InTransit,
            ReorderPoint = request.ReorderPoint,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Guid id, Inventory inventory, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != inventory.Id)
            return false;

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        existing.ProductItemId = inventory.ProductItemId;
        existing.LocationId = inventory.LocationId;
        existing.OnHand = inventory.OnHand;
        existing.Reserved = inventory.Reserved;
        existing.InTransit = inventory.InTransit;
        existing.ReorderPoint = inventory.ReorderPoint;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        repo.Update(existing);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        repo.Remove(existing);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}