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
        RuleForEach(x => x.PricingRules).SetValidator(new BundlePricingRuleDtoValidator());
    }
}

public class BundleItemDtoValidator : AbstractValidator<BundleItemDto>
{
    public BundleItemDtoValidator()
    {
        RuleFor(x => x.ChildId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

public class BundlePricingRuleDtoValidator : AbstractValidator<BundlePricingRuleDto>
{
    public BundlePricingRuleDtoValidator()
    {
        RuleFor(x => x.RuleType)
            .IsInEnum();

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).When(x => x.Amount.HasValue);

        RuleFor(x => x.PercentOff)
            .InclusiveBetween(0, 100).When(x => x.PercentOff.HasValue);

        RuleFor(x => x.ApplyTo)
            .IsInEnum();
    }
}