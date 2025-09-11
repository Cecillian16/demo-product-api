using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class LocationService(ILocationRepository repo) : ILocationService
{
    public Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default) =>
        repo.GetAllAsync(ct);

    public Task<Location?> GetAsync(Guid id, CancellationToken ct = default) =>
        repo.GetByIdAsync(id, asTracking: false, ct);

    public async Task<Location> CreateAsync(LocationCreateRequest request, CancellationToken ct = default)
    {
        var entity = new Location
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Address1 = request.Address1,
            Address2 = request.Address2,
            City = request.City,
            Country = request.Country
        };
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Guid id, Location location, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != location.Id)
            return false;

        var existing = await repo.GetByIdAsync(id, asTracking: true, ct);
        if (existing is null) return false;

        existing.Name = location.Name;
        existing.Type = location.Type;
        existing.Address1 = location.Address1;
        existing.Address2 = location.Address2;
        existing.City = location.City;
        existing.Country = location.Country;

        repo.Update(existing);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, asTracking: true, ct);
        if (existing is null) return false;

        repo.Remove(existing);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}