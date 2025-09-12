using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProductApi.Application.Repositories;

public interface IBundleRepository
{
    Task BulkInsert(IEnumerable<Bundle> bundles);
}
