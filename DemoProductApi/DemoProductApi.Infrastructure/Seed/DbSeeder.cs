using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductApi.Infrastructure.Seed;

public static class DbSeeder
{
    // PRODUCT + VARIANTS + ITEMS
    private static readonly Guid ProductId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid VariantOptionColorId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid VariantValueRedId = Guid.Parse("20000000-0000-0000-0000-000000000101");
    private static readonly Guid VariantValueBlueId = Guid.Parse("20000000-0000-0000-0000-000000000102");
    private static readonly Guid ProductItemRedId = Guid.Parse("30000000-0000-0000-0000-000000000101");
    private static readonly Guid ProductItemBlueId = Guid.Parse("30000000-0000-0000-0000-000000000102");

    // LOCATIONS
    private static readonly Guid MainWarehouseId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid StoreFrontId   = Guid.Parse("40000000-0000-0000-0000-000000000002");

    // INVENTORIES
    private static readonly Guid InventoryRedMainId  = Guid.Parse("50000000-0000-0000-0000-000000000101");
    private static readonly Guid InventoryBlueMainId = Guid.Parse("50000000-0000-0000-0000-000000000102");
    private static readonly Guid InventoryRedStoreId = Guid.Parse("50000000-0000-0000-0000-000000000103");

    // PRICES
    private static readonly Guid PriceProductBaseId     = Guid.Parse("60000000-0000-0000-0000-000000000001");
    private static readonly Guid PriceItemRedId         = Guid.Parse("60000000-0000-0000-0000-000000000101");
    private static readonly Guid PriceItemBlueId        = Guid.Parse("60000000-0000-0000-0000-000000000102");
    private static readonly Guid PriceBundleStarterId   = Guid.Parse("60000000-0000-0000-0000-000000000201");

    // BUNDLES
    private static readonly Guid BundleStarterId  = Guid.Parse("70000000-0000-0000-0000-000000000001");
    private static readonly Guid BundleFamilyId   = Guid.Parse("70000000-0000-0000-0000-000000000002");
    private static readonly Guid BundlePricingRuleId = Guid.Parse("71000000-0000-0000-0000-000000000001");

    // (If you use another enum name adjust PriceEntityType.* / Status.* / rule enums accordingly)
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // PRODUCT
        if (!await db.Products.AnyAsync(p => p.ProductId == ProductId, ct))
        {
            db.Products.Add(new Product
            {
                ProductId = ProductId,
                Name = "T-Shirt",
                SkuPrefix = "TSH",
                Status = Status.Active,
                Description = "Demo seeded T-Shirt product",
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // VARIANT OPTION (Color)
        if (!await db.VariantOptions.AnyAsync(v => v.VariantOptionId == VariantOptionColorId, ct))
        {
            db.VariantOptions.Add(new VariantOption
            {
                VariantOptionId = VariantOptionColorId,
                ProductId = ProductId,
                Name = "Color"
            });
        }

        // VARIANT OPTION VALUES
        if (!await db.VariantOptionValues.AnyAsync(v => v.VariantOptionValueId == VariantValueRedId, ct))
        {
            db.VariantOptionValues.AddRange(
                new VariantOptionValue
                {
                    VariantOptionValueId = VariantValueRedId,
                    VariantOptionId = VariantOptionColorId,
                    Value = "Red",
                    Code = "RED"
                },
                new VariantOptionValue
                {
                    VariantOptionValueId = VariantValueBlueId,
                    VariantOptionId = VariantOptionColorId,
                    Value = "Blue",
                    Code = "BLUE"
                });
        }

        // PRODUCT ITEMS (SKU variants)
        if (!await db.ProductItems.AnyAsync(i => i.ProductItemId == ProductItemRedId, ct))
        {
            db.ProductItems.AddRange(
                new ProductItem
                {
                    ProductItemId = ProductItemRedId,
                    ProductId = ProductId,
                    Sku = "TSH-RED-M",
                    Barcode = "BAR-RED-M",
                    Status = Status.Active,
                    Weight = 0.25m,
                    Volume = 0.0015m,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ProductItem
                {
                    ProductItemId = ProductItemBlueId,
                    ProductId = ProductId,
                    Sku = "TSH-BLU-M",
                    Barcode = "BAR-BLU-M",
                    Status = Status.Active,
                    Weight = 0.25m,
                    Volume = 0.0015m,
                    CreatedAt = now,
                    UpdatedAt = now
                });
        }

        // PRODUCT ITEM VARIANT VALUES (bridge)
        if (!await db.ProductItemVariantValues.AnyAsync(x => x.ProductItemId == ProductItemRedId && x.VariantOptionId == VariantOptionColorId, ct))
        {
            db.ProductItemVariantValues.AddRange(
                new ProductItemVariantValue
                {
                    ProductItemId = ProductItemRedId,
                    VariantOptionId = VariantOptionColorId,
                    VariantOptionValueId = VariantValueRedId
                },
                new ProductItemVariantValue
                {
                    ProductItemId = ProductItemBlueId,
                    VariantOptionId = VariantOptionColorId,
                    VariantOptionValueId = VariantValueBlueId
                });
        }

        // LOCATIONS
        if (!await db.Locations.AnyAsync(l => l.Id == MainWarehouseId, ct))
        {
            db.Locations.AddRange(
                new Location
                {
                    Id = MainWarehouseId,
                    Name = "Main Warehouse",
                    Type = "WH",
                    Address1 = "100 Logistics Ave",
                    City = "Metropolis",
                    Country = "US"
                },
                new Location
                {
                    Id = StoreFrontId,
                    Name = "Retail Store",
                    Type = "STORE",
                    Address1 = "200 Commerce St",
                    City = "Metropolis",
                    Country = "US"
                });
        }

        // INVENTORIES
        if (!await db.Inventories.AnyAsync(i => i.Id == InventoryRedMainId, ct))
        {
            db.Inventories.AddRange(
                new Inventory
                {
                    Id = InventoryRedMainId,
                    ProductItemId = ProductItemRedId,
                    LocationId = MainWarehouseId,
                    OnHand = 120,
                    Reserved = 5,
                    InTransit = 20,
                    ReorderPoint = 40,
                    UpdatedAt = now
                },
                new Inventory
                {
                    Id = InventoryBlueMainId,
                    ProductItemId = ProductItemBlueId,
                    LocationId = MainWarehouseId,
                    OnHand = 90,
                    Reserved = 3,
                    InTransit = 10,
                    ReorderPoint = 30,
                    UpdatedAt = now
                },
                new Inventory
                {
                    Id = InventoryRedStoreId,
                    ProductItemId = ProductItemRedId,
                    LocationId = StoreFrontId,
                    OnHand = 25,
                    Reserved = 2,
                    InTransit = 0,
                    ReorderPoint = 10,
                    UpdatedAt = now
                });
        }

        // PRICES (assuming PriceEntityType.Product / ProductItem / Bundle exist)
        if (!await db.Prices.AnyAsync(p => p.Id == PriceProductBaseId, ct))
        {
            db.Prices.AddRange(
                new Price
                {
                    Id = PriceProductBaseId,
                    EntityType = PriceEntityType.Product,
                    EntityId = ProductId,
                    Currency = "USD",
                    ListPrice = 25.00m,
                    SalePrice = 22.50m,
                    ValidFrom = today
                },
                new Price
                {
                    Id = PriceItemRedId,
                    EntityType = PriceEntityType.Item,
                    EntityId = ProductItemRedId,
                    Currency = "USD",
                    ListPrice = 26.00m,
                    SalePrice = 24.00m,
                    ValidFrom = today
                },
                new Price
                {
                    Id = PriceItemBlueId,
                    EntityType = PriceEntityType.Item,
                    EntityId = ProductItemBlueId,
                    Currency = "USD",
                    ListPrice = 26.00m,
                    ValidFrom = today
                });
        }

        // BUNDLES
        if (!await db.Bundles.AnyAsync(bu => bu.BundleId == BundleStarterId, ct))
        {
            db.Bundles.AddRange(
                new Bundle
                {
                    BundleId = BundleStarterId,
                    Name = "Starter Pack",
                    Description = "Red shirt bundle",
                    Status = Status.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Bundle
                {
                    BundleId = BundleFamilyId,
                    Name = "Family Pack",
                    Description = "Starter + Blue variant",
                    Status = Status.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                });
        }

        // BUNDLE ITEMS (Starter contains Red item; Family contains Starter + Blue item)
        if (!await db.BundleItems.AnyAsync(bi => bi.BundleId == BundleStarterId, ct))
        {
            db.BundleItems.AddRange(
                new BundleItem
                {
                    BundleId = BundleStarterId,
                    ChildId = ProductItemRedId, // product item
                    Quantity = 1m
                },
                new BundleItem
                {
                    BundleId = BundleFamilyId,
                    ChildId = BundleStarterId, // child bundle
                    Quantity = 1m
                },
                new BundleItem
                {
                    BundleId = BundleFamilyId,
                    ChildId = ProductItemBlueId, // product item
                    Quantity = 1m
                });
        }

        // BUNDLE PRICING RULE (Example: 10% off list)
        if (!await db.BundlePricingRules.AnyAsync(r => r.BundlePricingRuleId == BundlePricingRuleId, ct))
        {
            db.BundlePricingRules.Add(new BundlePricingRule
            {
                BundlePricingRuleId = BundlePricingRuleId,
                BundleId = BundleFamilyId,
                RuleType = BundlePricingRuleType.PercentOff,
                PercentOff = 10m,
                ApplyTo = ApplyToScope.RequiredOnly
            });
        }

        // Bundle price (after creating bundle rows so foreign relationships exist logically)
        if (!await db.Prices.AnyAsync(p => p.Id == PriceBundleStarterId, ct))
        {
            db.Prices.Add(new Price
            {
                Id = PriceBundleStarterId,
                EntityType = PriceEntityType.Item,
                EntityId = BundleStarterId,
                Currency = "USD",
                ListPrice = 48.00m,
                SalePrice = 45.00m,
                ValidFrom = today
            });
        }

        if (db.ChangeTracker.HasChanges())
            await db.SaveChangesAsync(ct);
    }
}

public static class MigrationAndSeedExtensions
{
    public static async Task MigrateAndSeedAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(ct);
        await DbSeeder.SeedAsync(db, ct);
    }
}