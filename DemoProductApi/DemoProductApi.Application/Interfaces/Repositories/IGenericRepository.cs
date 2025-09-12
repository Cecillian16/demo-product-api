using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Application.Repositories;

public interface IGenericRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task Update(T existing, T replacement, CancellationToken ct = default);
    void Remove(T entity);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
