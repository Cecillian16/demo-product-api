using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class PriceRepository(AppDbContext db) : IGenericRepository<Price>
{
    public async Task<IReadOnlyList<Price>> GetAllAsync(CancellationToken ct = default) =>
        await db.Prices.AsNoTracking().ToListAsync(ct);

    public async Task<Price?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Prices.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Price price, CancellationToken ct = default) =>
        await db.Prices.AddAsync(price, ct);

    public void Update(Price price) =>
        db.Prices.Update(price);

    public void Remove(Price price) =>
        db.Prices.Remove(price);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    => db.Database.BeginTransactionAsync(ct);
}