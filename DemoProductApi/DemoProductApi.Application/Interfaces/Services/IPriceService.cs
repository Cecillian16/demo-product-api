using DemoProductApi.Domain.Entities;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Interfaces.Services;

public interface IPriceService
{
    Task<IReadOnlyList<Price>> GetAllAsync(CancellationToken ct = default);
    Task<Price?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Price> CreateAsync(PriceCreateRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Price price, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}