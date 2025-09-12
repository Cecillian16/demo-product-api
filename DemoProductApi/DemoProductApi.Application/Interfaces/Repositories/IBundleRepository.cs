using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Application.Repositories;

public interface IBundleRepository
{
    Task AddRangeAsync(IEnumerable<Bundle> bundles, CancellationToken ct = default);
    Task InsertBatch(IEnumerable<Bundle> bundles);
}
