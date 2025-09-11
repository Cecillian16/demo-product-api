using FluentValidation;
using DemoProductApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DemoProductApi.Application.Models;

namespace DemoProductApi.Validators;

public class ProductItemDtoValidator : AbstractValidator<ProductItemDto>
{
    public ProductItemDtoValidator(AppDbContext db)
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.VariantValues)
            .NotNull()
            .Must(list => list.Count > 0)
            .WithMessage("At least one variant value must be provided.")
            .Must(list => list.Select(v => v.VariantOptionId).Distinct().Count() == list.Count)
            .WithMessage("Duplicate VariantOptionId entries are not allowed.");

        // Ensure all VariantOptionIds exist
        RuleFor(x => x)
            .MustAsync(async (dto, ct) =>
            {
                var optionIds = dto.VariantValues.Select(v => v.VariantOptionId).Distinct().ToList();
                var count = await db.VariantOptions
                    .CountAsync(o => optionIds.Contains(o.VariantOptionId), ct);
                return count == optionIds.Count;
            })
            .WithMessage("One or more VariantOptionId values do not exist.");

        // Ensure all VariantOptionValueIds exist
        RuleFor(x => x)
            .MustAsync(async (dto, ct) =>
            {
                var valueIds = dto.VariantValues.Select(v => v.VariantOptionValueId).Distinct().ToList();
                var count = await db.VariantOptionValues
                    .CountAsync(v => valueIds.Contains(v.VariantOptionValueId), ct);
                return count == valueIds.Count;
            })
            .WithMessage("One or more VariantOptionValueId values do not exist.");

        // Ensure each VariantOptionValue belongs to its VariantOption
        RuleFor(x => x)
            .MustAsync(async (dto, ct) =>
            {
                // Build needed pairs
                var pairs = dto.VariantValues
                    .Select(v => new { v.VariantOptionId, v.VariantOptionValueId })
                    .ToList();

                // Fetch all values for those options
                var optionIds = pairs.Select(p => p.VariantOptionId).Distinct().ToList();
                var values = await db.VariantOptionValues
                    .Where(v => optionIds.Contains(v.VariantOptionId))
                    .Select(v => new { v.VariantOptionId, v.VariantOptionValueId })
                    .ToListAsync(ct);

                var set = values.ToHashSet();

                return pairs.All(p => set.Contains(p));
            })
            .WithMessage("One or more VariantOptionValueId entries do not belong to their VariantOptionId.");

        // Per-item rules (basic non-empty)
        RuleForEach(x => x.VariantValues)
            .SetValidator(new ProductItemVariantValueDtoValidator());
    }
}

public class ProductItemVariantValueDtoValidator : AbstractValidator<ProductItemVariantValueDto>
{
    public ProductItemVariantValueDtoValidator()
    {
        RuleFor(x => x.VariantOptionId)
            .NotEmpty();

        RuleFor(x => x.VariantOptionValueId)
            .NotEmpty();
    }
}