using DemoProductApi.Application.Models;
using FluentValidation;

namespace DemoProductApi.Validators;

public class BundleDtoValidator : AbstractValidator<BundleDto>
{
    public BundleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Items)
            .NotNull().Must(items => items.Count > 0)
            .WithMessage("At least one bundle item is required.");

        RuleForEach(x => x.Items).SetValidator(new BundleItemDtoValidator());
    }
}

public class BundleItemDtoValidator : AbstractValidator<BundleItemDto>
{
    public BundleItemDtoValidator()
    {
        RuleFor(x => x.ChildProductItemId)
            .NotNull()
            .WithMessage("Either ChildProductItemId must be provided.");

        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}