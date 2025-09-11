using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface IPriceRepository
{
    Task<IReadOnlyList<Price>> GetAllAsync(CancellationToken ct = default);

    Task<Price?> GetByIdAsync(Guid id, bool asTracking = false, CancellationToken ct = default);

    /// <summary>
    /// Returns currently active prices for a single entity (optionally filtered by currency).
    /// </summary>
    Task<IReadOnlyList<Price>> GetActiveAsync(
        PriceEntityType entityType,
        Guid entityId,
        DateOnly onDate,
        string? currency = null,
        CancellationToken ct = default);

    /// <summary>
    /// Returns historical (all) prices for an entity ordered descending by ValidFrom (optionally limited).
    /// </summary>
    Task<IReadOnlyList<Price>> GetHistoryAsync(
        PriceEntityType entityType,
        Guid entityId,
        int? top = null,
        CancellationToken ct = default);

    Task AddAsync(Price price, CancellationToken ct = default);
    void Update(Price price);
    void Remove(Price price);
    Task SaveChangesAsync(CancellationToken ct = default);
}