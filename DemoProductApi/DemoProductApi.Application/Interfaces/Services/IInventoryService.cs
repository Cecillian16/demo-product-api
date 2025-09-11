using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<Inventory>> GetAllAsync(CancellationToken ct = default);
    Task<Inventory?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Inventory> CreateAsync(InventoryCreateRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Inventory inventory, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}