using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Services;
using DemoProductApi.Infrastructure;
using DemoProductApi.Infrastructure.Seed;
using DemoProductApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace DemoProductApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssemblyContaining<BundleDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ProductItemDtoValidator>();
            // Added to allow validators to inspect route values
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddScoped<IDbConnection>(sp =>
                new NpgsqlConnection(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddScoped<IBundleService, BundleService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductItemService, ProductItemService>();
            //builder.Services.AddScoped<IPriceService, PriceService>();
            //builder.Services.AddScoped<IInventoryService, InventoryService>();
            //builder.Services.AddScoped<ILocationService, LocationService>();

            builder.Services.AddInfrastructureServices();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply migrations + seed
            await app.Services.MigrateAndSeedAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
