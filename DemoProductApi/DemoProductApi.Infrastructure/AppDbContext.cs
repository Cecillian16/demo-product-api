// Infrastructure/AppDbContext.cs
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<VariantOption> VariantOptions => Set<VariantOption>();
    public DbSet<VariantOptionValue> VariantOptionValues => Set<VariantOptionValue>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();
    public DbSet<ProductItemVariantValue> ProductItemVariantValues => Set<ProductItemVariantValue>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Bundle> Bundles => Set<Bundle>();
    public DbSet<BundleItem> BundleItems => Set<BundleItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SkuPrefix).HasMaxLength(50);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => new { x.Status, x.Name });
        });

        b.Entity<VariantOption>(e =>
        {
            e.HasKey(x => x.VariantOptionId);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Product)
             .WithMany(p => p.VariantOptions)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<VariantOptionValue>(e =>
        {
            e.HasKey(x => x.VariantOptionValueId);
            e.Property(x => x.Value).HasMaxLength(80).IsRequired();
            e.Property(x => x.Code).HasMaxLength(40);
            e.HasIndex(x => new { x.VariantOptionId, x.Value }).IsUnique();
            e.HasOne(x => x.VariantOption)
             .WithMany(o => o.Values)
             .HasForeignKey(x => x.VariantOptionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<ProductItem>(e =>
        {
            e.HasKey(x => x.ProductItemId);
            e.Property(x => x.Sku).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.Weight).HasPrecision(18, 3);
            e.Property(x => x.Volume).HasPrecision(18, 3);
            e.HasOne(x => x.Product)
             .WithMany(p => p.ProductItems)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<ProductItemVariantValue>(e =>
        {
            e.HasKey(x => new { x.ProductItemId, x.VariantOptionId });
            e.HasOne(x => x.ProductItem)
             .WithMany(i => i.VariantValues)
             .HasForeignKey(x => x.ProductItemId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.VariantOption)
             .WithMany()
             .HasForeignKey(x => x.VariantOptionId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.VariantOptionValue)
             .WithMany(v => v.ProductItemVariantValues)
             .HasForeignKey(x => x.VariantOptionValueId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.VariantOptionId, x.VariantOptionValueId });
        });

        b.Entity<Location>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        b.Entity<Inventory>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.ProductItemId, x.LocationId }).IsUnique();
            e.HasOne(x => x.ProductItem)
             .WithMany(i => i.Inventories)
             .HasForeignKey(x => x.ProductItemId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Location)
             .WithMany(l => l.Inventories)
             .HasForeignKey(x => x.LocationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Price>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.EntityType).HasConversion<int>();
            e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            e.Property(x => x.ListPrice).HasPrecision(18, 2);
            e.Property(x => x.SalePrice).HasPrecision(18, 2);
            e.HasIndex(x => new { x.EntityType, x.EntityId, x.ValidFrom, x.ValidTo });
        });

        b.Entity<Bundle>(e =>
        {
            e.HasKey(x => x.BundleId);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => new { x.Status, x.Name });
        });

        b.Entity<BundleItem>(e =>
        {
            e.HasKey(b => b.Id);

            e.HasOne(b => b.Bundle)
                .WithMany(bu => bu.Items)
                .HasForeignKey(b => b.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(b => b.ChildProductItem)
                .WithMany()
                .HasForeignKey(b => b.ChildProductItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
