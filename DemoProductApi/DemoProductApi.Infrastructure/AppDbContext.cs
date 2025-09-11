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
    public DbSet<BundlePricingRule> BundlePricingRules => Set<BundlePricingRule>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // PRODUCT
        b.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SkuPrefix).HasMaxLength(50);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => new { x.Status, x.Name });
        });

        // VARIANT OPTION
        b.Entity<VariantOption>(e =>
        {
            e.HasKey(x => x.VariantOptionId);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Product)
             .WithMany(p => p.VariantOptions)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // VARIANT OPTION VALUE
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

        // PRODUCT ITEM (SKU)
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

        // PRODUCT ITEM VARIANT VALUE (bridge)
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

            // Optional helper index to filter by option/value quickly
            e.HasIndex(x => new { x.VariantOptionId, x.VariantOptionValueId });
        });

        // LOCATION
        b.Entity<Location>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        // INVENTORY
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

        // PRICE
        b.Entity<Price>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.EntityType).HasConversion<int>();
            e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            e.Property(x => x.ListPrice).HasPrecision(18, 2);
            e.Property(x => x.SalePrice).HasPrecision(18, 2);
            e.HasIndex(x => new { x.EntityType, x.EntityId, x.ValidFrom, x.ValidTo });
        });

        // BUNDLE
        b.Entity<Bundle>(e =>
        {
            e.HasKey(x => x.BundleId);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => new { x.Status, x.Name });
        });

        // BUNDLE ITEM (supports child ProductItem or Bundle)
        b.Entity<BundleItem>(e =>
        {
            e.HasKey(x => new { x.BundleId, x.ChildId });
            e.Property(x => x.Quantity).HasPrecision(18, 3);

            e.HasOne(x => x.Bundle)
             .WithMany(bu => bu.Items)
             .HasForeignKey(x => x.BundleId)
             .OnDelete(DeleteBehavior.Cascade);

            // Optional convenience navigations (no FK enforced by EF for polymorphic target)
            e.HasOne(x => x.ChildItem)
             .WithMany()
             .HasForeignKey(x => x.ChildId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);

            e.HasOne(x => x.ChildBundle)
             .WithMany()
             .HasForeignKey(x => x.ChildId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        // BUNDLE PRICING RULE
        b.Entity<BundlePricingRule>(e =>
        {
            e.HasKey(x => x.BundlePricingRuleId);
            e.Property(x => x.RuleType).HasConversion<int>();
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.PercentOff).HasPrecision(5, 2);
            e.Property(x => x.ApplyTo).HasConversion<int>();

            e.HasOne(x => x.Bundle)
             .WithMany(bu => bu.PricingRules)
             .HasForeignKey(x => x.BundleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Optional checks for data hygiene (supported by EF Core for PostgreSQL via Npgsql) ---
        // b.Entity<VariantOptionValue>()
        //     .ToTable(t => t.HasCheckConstraint("ck_vov_value_not_empty", "length(value) > 0"));
        //
        // NOTE: The strict rule “ProductItemVariantValue.VariantOption belongs to same Product as ProductItem”
        // is hard to enforce with a simple FK. Enforce in application service and/or DB trigger if critical.
    }
}
