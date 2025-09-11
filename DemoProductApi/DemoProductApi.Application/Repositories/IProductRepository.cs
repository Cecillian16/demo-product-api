using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId, bool includeDetails = false, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetPagedAsync(
        int page, int pageSize,
        string? search = null,
        Status? status = null,
        bool includeDetails = false,
        CancellationToken ct = default);

    Task<int> CountAsync(string? search = null, Status? status = null, CancellationToken ct = default);

    Task AddAsync(Product product, CancellationToken ct = default);
    void Update(Product product);
    void Remove(Product product);

    // Convenience richer fetch
    Task<Product?> GetWithInventoryAndPricesAsync(Guid productId, string? currency = null, DateOnly? onDate = null, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}