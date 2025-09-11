using DemoProductApi.Application.Models;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using System;

namespace DemoProductApi.Tests.Utils;

public static class TestBuilders
{
    public static Product NewProduct() => new()
    {
        ProductId = Guid.NewGuid(),
        Name = "Prod",
        SkuPrefix = "P",
        Description = "Desc",
        Status = Status.Active,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };

    public static ProductDto NewProductDto(Guid? id = null) => new()
    {
        ProductId = id ?? Guid.NewGuid(),
        Name = "Prod DTO",
        SkuPrefix = "PD",
        Description = "Dto Desc",
        Status = (int)Status.Active,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
        VariantOptions = new()
    };

    public static Bundle NewBundle() => new()
    {
        BundleId = Guid.NewGuid(),
        Name = "Bundle",
        Description = "Bundle Desc",
        Status = Status.Active,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };

    public static BundleDto NewBundleDto(Guid? id = null) => new()
    {
        BundleId = id ?? Guid.NewGuid(),
        Name = "Bundle DTO",
        Description = "Bundle DTO Desc",
        Status = (int)Status.Active,
        Items = new(),
        PricingRules = new()
    };

    public static ProductItem NewProductItem(Guid? productId = null) => new()
    {
        ProductItemId = Guid.NewGuid(),
        ProductId = productId ?? Guid.NewGuid(),
        Sku = "SKU-1",
        Status = Status.Active,
        Weight = 1.1m,
        Volume = 0.9m,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };

    public static ProductItemDto NewProductItemDto(Guid? id = null, Guid? productId = null) => new()
    {
        ProductItemId = id ?? Guid.NewGuid(),
        ProductId = productId ?? Guid.NewGuid(),
        Sku = "SKU-D",
        Status = (int)Status.Active,
        Weight = 3.3m,
        Volume = 2.2m,
        VariantValues = new()
    };

    public static Price NewPrice(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EntityType = PriceEntityType.Product,
        EntityId = Guid.NewGuid(),
        Currency = "USD",
        ListPrice = 50m,
        SalePrice = 40m,
        ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
        ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
    };

    public static Inventory NewInventory(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        ProductItemId = Guid.NewGuid(),
        LocationId = Guid.NewGuid(),
        OnHand = 10,
        Reserved = 2,
        InTransit = 1,
        ReorderPoint = 3,
        UpdatedAt = DateTimeOffset.UtcNow
    };

    public static Location NewLocation(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Location A",
        Type = "WH",
        City = "City",
        Country = "US"
    };
}