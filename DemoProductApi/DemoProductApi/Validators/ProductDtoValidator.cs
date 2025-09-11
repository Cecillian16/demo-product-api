using DemoProductApi.Application.Models;
using DemoProductApi.Domain;
using DemoProductApi.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DemoProductApi.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator(AppDbContext db)
    {
        // Basic fields
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SkuPrefix)
            .NotEmpty()
            .MaximumLength(32)
            .Matches("^[A-Z0-9_-]+$")
            .WithMessage("SkuPrefix must contain only A-Z, 0-9, underscore or dash.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status value is invalid.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        // Optional: ensure CreatedAt/UpdatedAt not in the future
        RuleFor(x => x.CreatedAt)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("CreatedAt cannot be in the future.");

        RuleFor(x => x.UpdatedAt)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("UpdatedAt cannot be in the future.");

        // VariantOptions collection rules
        RuleFor(x => x.VariantOptions)
            .NotNull();

        // No duplicate VariantOptionId
        RuleFor(x => x.VariantOptions)
            .Must(list => list.Select(o => o.VariantOptionId).Where(id => id != Guid.Empty).Distinct().Count() ==
                          list.Where(o => o.VariantOptionId != Guid.Empty).Count())
            .WithMessage("Duplicate VariantOptionId entries are not allowed.");

        // No duplicate VariantOption Name (case-insensitive)
        RuleFor(x => x.VariantOptions)
            .Must(list =>
            {
                var names = list.Select(o => o.Name.Trim().ToUpperInvariant());
                return names.Distinct().Count() == list.Count;
            })
            .WithMessage("Duplicate VariantOption Name entries are not allowed.");

        // Validate each VariantOption
        RuleForEach(x => x.VariantOptions)
            .SetValidator(new VariantOptionDtoValidator());

        // (Optional) DB-level uniqueness checks (uncomment if desired):
        // RuleFor(x => x)
        //     .MustAsync(async (dto, ct) =>
        //     {
        //         var exists = await db.Products
        //             .AnyAsync(p => p.ProductId != dto.ProductId && p.SkuPrefix == dto.SkuPrefix, ct);
        //         return !exists;
        //     })
        //     .WithMessage("Another product already uses this SkuPrefix.");
    }
}

public class VariantOptionDtoValidator : AbstractValidator<VariantOptionDto>
{
    public VariantOptionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Values)
            .NotNull()
            .Must(v => v.Count > 0)
            .WithMessage("Each variant option must have at least one value.");

        // No duplicate VariantOptionValueId
        RuleFor(x => x.Values)
            .Must(values =>
                values.Select(v => v.VariantOptionValueId)
                      .Where(id => id != Guid.Empty)
                      .Distinct().Count() ==
                values.Where(v => v.VariantOptionValueId != Guid.Empty).Count())
            .WithMessage("Duplicate VariantOptionValueId entries are not allowed.");

        // No duplicate textual value (case-insensitive)
        RuleFor(x => x.Values)
            .Must(values =>
            {
                var texts = values.Select(v => v.Value.Trim().ToUpperInvariant());
                return texts.Distinct().Count() == values.Count;
            })
            .WithMessage("Duplicate VariantOptionValue Value entries are not allowed.");

        RuleForEach(x => x.Values)
            .SetValidator(new VariantOptionValueDtoValidator());
    }
}

public class VariantOptionValueDtoValidator : AbstractValidator<VariantOptionValueDto>
{
    public VariantOptionValueDtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Code)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.Code));
    }
}