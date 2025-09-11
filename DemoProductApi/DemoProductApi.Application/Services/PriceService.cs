using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Services;

public class PriceService(IPriceRepository repo) : IPriceService
{
    public Task<IReadOnlyList<Price>> GetAllAsync(CancellationToken ct = default) =>
        repo.GetAllAsync(ct);

    public Task<Price?> GetAsync(Guid id, CancellationToken ct = default) =>
        repo.GetByIdAsync(id, asTracking: false, ct);

    public async Task<Price> CreateAsync(PriceCreateRequest request, CancellationToken ct = default)
    {
        var entity = new Price
        {
            Id = Guid.NewGuid(),
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Currency = request.Currency,
            ListPrice = request.ListPrice,
            SalePrice = request.SalePrice,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Guid id, Price price, CancellationToken ct = default)
    {
        if (id == Guid.Empty || id != price.Id)
            return false;

        var existing = await repo.GetByIdAsync(id, asTracking: true, ct);
        if (existing is null) return false;

        existing.EntityType = price.EntityType;
        existing.EntityId = price.EntityId;
        existing.Currency = price.Currency;
        existing.ListPrice = price.ListPrice;
        existing.SalePrice = price.SalePrice;
        existing.ValidFrom = price.ValidFrom;
        existing.ValidTo = price.ValidTo;

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