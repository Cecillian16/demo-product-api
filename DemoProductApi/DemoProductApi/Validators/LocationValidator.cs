using DemoProductApi.Domain.Entities;
using DemoProductApi.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Validators;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator(AppDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Type));

        RuleFor(x => x.Address1)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Address1));

        RuleFor(x => x.Address2)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Address2));

        RuleFor(x => x.City)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.Country)
            .Length(2, 3)
            .Matches("^[A-Z]{2,3}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .WithMessage("Country must be 2–3 uppercase letters.");

        // Unique name (optional – remove if duplicates allowed)
        RuleFor(x => x)
            .MustAsync(async (loc, ct) =>
            {
                var exists = await db.Locations
                    .Where(l => l.Name == loc.Name && l.Id != loc.Id)
                    .AnyAsync(ct);
                return !exists;
            })
            .WithMessage("A location with this Name already exists.");
    }
}