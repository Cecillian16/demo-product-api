using DemoProductApi.Application.Repositories;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IGenericRepository<Product>, ProductRepository>();
        services.AddScoped<IGenericRepository<Bundle>, BundleRepository>();
        services.AddScoped<IGenericRepository<ProductItem>, ProductItemRepository>();
        //services.AddScoped<IGenericRepository<Price>, PriceRepository>();
        //services.AddScoped<IGenericRepository<Inventory>, InventoryRepository>();
        //services.AddScoped<IGenericRepository<Location>, LocationRepository>();
        services.AddScoped<IBundleRepository, BundleRepository>();
        return services;
    }
}