using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class BundleService(IGenericRepository<Bundle> repo, IBundleRepository brepo) : IBundleService
{
    public async Task<IReadOnlyList<BundleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await repo.GetAllAsync(ct);
        return entities.Select(BundleMapper.ToDto).ToList();
    }

    public async Task<BundleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? null : BundleMapper.ToDto(entity);
    }

    public async Task<BundleDto> CreateAsync(BundleCreateRequest request, CancellationToken ct = default)
    {
        var entity = BundleMapper.ToEntity(request, Guid.Empty);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return BundleMapper.ToDto(entity);
    }

    public async Task<List<BundleDto>> BulkCreateAsync(List<BundleCreateRequest> requests, CancellationToken ct = default)
    {
        var entities = new List<Bundle>();
        foreach (var request in requests)
        {
            entities.Add(BundleMapper.ToEntity(request, Guid.Empty));
        }

        await brepo.BulkInsert(entities);

        var dtos = new List<BundleDto>();
        foreach (var entity in entities)
        {
            dtos.Add(BundleMapper.ToDto(entity));
        }
        return dtos;
    }

    public async Task<bool> UpdateAsync(Guid id, BundleCreateRequest request, CancellationToken ct = default)
    {
        if (id == Guid.Empty) return false;

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        var replacement = BundleMapper.ToEntity(request, id);

        await repo.Update(existing, replacement, ct);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        repo.Remove(entity);
        await repo.SaveChangesAsync(ct);
        return true;
    }
}