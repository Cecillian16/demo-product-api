using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;

namespace DemoProductApi.Application.Interfaces.Services;

public interface IBundleService
{
    Task<IReadOnlyList<BundleDto>> GetAllAsync(CancellationToken ct = default);
    Task<BundleDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<BundleDto> CreateAsync(BundleCreateRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, BundleCreateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}