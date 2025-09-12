using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Interfaces.Services;

public interface IProductItemService
{
    Task<IReadOnlyList<ProductItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductItemDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<ProductItemDto> CreateAsync(ProductItemCreateRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, ProductItemCreateRequest dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}