using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Interfaces.Services;

public interface ILocationService
{
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default);
    Task<Location?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Location> CreateAsync(LocationCreateRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Location location, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}