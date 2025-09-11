using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Repositories;

public interface IProductItemRepository
{
    Task<ProductItem?> GetByIdAsync(Guid productItemId, bool includeDetails = false, CancellationToken ct = default);
    Task<ProductItem?> GetBySkuAsync(string sku, bool includeDetails = false, CancellationToken ct = default);
    Task<IReadOnlyList<ProductItem>> GetByProductAsync(Guid productId, bool includeDetails = false, CancellationToken ct = default);

    Task AddAsync(ProductItem item, CancellationToken ct = default);
    void Update(ProductItem item);
    void Remove(ProductItem item);
    Task SaveChangesAsync(CancellationToken ct = default);
}