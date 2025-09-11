using DemoProductApi.Domain.Entities;
using DemoProductApi.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Validators;

public class PriceValidator : AbstractValidator<Price>
{
    public PriceValidator(AppDbContext db)
    {
        RuleFor(x => x.EntityType)
            .IsInEnum();

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a 3-letter ISO code (uppercase).");

        RuleFor(x => x.ListPrice)
            .GreaterThan(0);

        RuleFor(x => x.SalePrice)
            .GreaterThan(0)
            .LessThanOrEqualTo(x => x.ListPrice)
            .When(x => x.SalePrice.HasValue);

        // Optional uniqueness: only one Price per (EntityType, IdTarget)
        // (Assuming you have columns to distinguish the priced entity; adjust accordingly)
        RuleFor(x => x)
            .MustAsync(async (price, ct) =>
            {
                // If Id is empty (create) always check; if update ignore itself
                var already = await db.Prices
                    .Where(p => p.EntityType == price.EntityType
                                // Add additional matching criteria (e.g. p.TargetId) if exists
                                && p.Id != price.Id)
                    .AnyAsync(ct);

                return !already;
            })
            .WithMessage("A price for this entity already exists.");
    }
}