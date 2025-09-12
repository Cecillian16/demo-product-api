using Dapper;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace DemoProductApi.Infrastructure.Repositories;

public sealed class BundleRepository(AppDbContext db, IDbConnection conn) : IGenericRepository<Bundle>, IBundleRepository
{
    public async Task<IReadOnlyList<Bundle>> GetAllAsync(CancellationToken ct = default)
    {
        IQueryable<Bundle> q = db.Bundles;
        q = q.Include(b => b.Items);
        return await q.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Bundle?> GetByIdAsync(Guid bundleId, CancellationToken ct = default)
    {
        IQueryable<Bundle> q = db.Bundles;
        q = q.Include(b => b.Items);
        return await q.FirstOrDefaultAsync(b => b.BundleId == bundleId, ct);
    }

    public async Task AddAsync(Bundle bundle, CancellationToken ct = default)
        => await db.Bundles.AddAsync(bundle, ct);

    public async Task BulkInsert(IEnumerable<Bundle> bundles)
    {
        var list = bundles as IList<Bundle> ?? bundles.ToList();
        if (list == null || list.Count == 0) return;

        var bundleSql = new StringBuilder();
        bundleSql.Append("INSERT INTO public.\"Bundles\" (\"BundleId\",\"Name\",\"Description\",\"Status\",\"CreatedAt\",\"UpdatedAt\") VALUES ");

        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0) bundleSql.Append(',');
            bundleSql.AppendFormat("(@b{0},@n{0},@d{0},@s{0},@c{0},@u{0})", i);
        }

        var bundleParams = new DynamicParameters();
        for (int i = 0; i < list.Count; i++)
        {
            var b = list[i];
            bundleParams.Add($"b{i}", b.BundleId);
            bundleParams.Add($"n{i}", b.Name);
            bundleParams.Add($"d{i}", b.Description);
            bundleParams.Add($"s{i}", (int)b.Status);
            bundleParams.Add($"c{i}", b.CreatedAt);
            bundleParams.Add($"u{i}", b.UpdatedAt);
        }

        var allItems = list
            .Where(b => b.Items != null && b.Items.Count > 0)
            .SelectMany(b => b.Items.Select(it =>
            {
                if (it.Id == Guid.Empty) it.Id = Guid.NewGuid();
                it.BundleId = b.BundleId;
                return it;
            }))
            .ToList();

        StringBuilder? itemsSql = null;
        DynamicParameters? itemsParams = null;

        if (allItems.Count > 0)
        {
            itemsSql = new StringBuilder();
            itemsSql.Append("INSERT INTO public.\"BundleItems\" (\"Id\",\"BundleId\",\"ChildProductItemId\",\"Quantity\") VALUES ");
            itemsParams = new DynamicParameters();
            for (int i = 0; i < allItems.Count; i++)
            {
                if (i > 0) itemsSql.Append(',');
                itemsSql.AppendFormat("(@ib{0},@bb{0},@cp{0},@q{0})", i);

                var it = allItems[i];
                itemsParams.Add($"ib{i}", it.Id);
                itemsParams.Add($"bb{i}", it.BundleId);
                itemsParams.Add($"cp{i}", it.ChildProductItemId);
                itemsParams.Add($"q{i}", it.Quantity);
            }
        }

        if (conn.State != ConnectionState.Open)
            conn.Open();

        using var tx = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(bundleSql!.ToString(), bundleParams, tx);
            if (allItems.Count > 0 && itemsSql != null && itemsParams != null)
                await conn.ExecuteAsync(itemsSql.ToString(), itemsParams, tx);
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task Update(Bundle existing, Bundle replacement, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        try
        {
            db.Bundles.Remove(existing);
            await db.SaveChangesAsync(ct);

            await db.Bundles.AddAsync(replacement, ct);
            await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public void Remove(Bundle bundle)
        => db.Bundles.Remove(bundle);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => db.Database.BeginTransactionAsync(ct);
}