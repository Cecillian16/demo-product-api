using DemoProductApi.Application.Repositories;
using DemoProductApi.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBundleRepository, BundleRepository>();
        services.AddScoped<IProductItemRepository, ProductItemRepository>();
        services.AddScoped<IPriceRepository, PriceRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        return services;
    }
}