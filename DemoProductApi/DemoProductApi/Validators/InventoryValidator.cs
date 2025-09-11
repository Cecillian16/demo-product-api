using DemoProductApi.Domain.Entities;
using DemoProductApi.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Validators;

public class InventoryValidator : AbstractValidator<Inventory>
{
    public InventoryValidator(AppDbContext db)
    {
        RuleFor(x => x.ProductItemId)
            .NotEmpty();

        RuleFor(x => x.LocationId)
            .NotEmpty();

        RuleFor(x => x.OnHand)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Reserved)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.OnHand)
            .WithMessage("Reserved cannot exceed OnHand.");

        RuleFor(x => x.InTransit)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ReorderPoint)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.UpdatedAt)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("UpdatedAt cannot be in the future.");

        // Ensure referenced ProductItem exists
        RuleFor(x => x.ProductItemId)
            .MustAsync(async (id, ct) =>
                await db.ProductItems.AnyAsync(pi => pi.ProductItemId == id, ct))
            .WithMessage("Referenced ProductItem does not exist.");

        // Ensure referenced Location exists
        RuleFor(x => x.LocationId)
            .MustAsync(async (id, ct) =>
                await db.Locations.AnyAsync(l => l.Id == id, ct))
            .WithMessage("Referenced Location does not exist.");

        // Uniqueness: one inventory record per (ProductItemId, LocationId)
        RuleFor(x => x)
            .MustAsync(async (inv, ct) =>
            {
                var exists = await db.Inventories
                    .Where(i => i.ProductItemId == inv.ProductItemId
                                && i.LocationId == inv.LocationId
                                && i.Id != inv.Id)
                    .AnyAsync(ct);
                return !exists;
            })
            .WithMessage("Inventory for this ProductItem and Location already exists.");
    }
}