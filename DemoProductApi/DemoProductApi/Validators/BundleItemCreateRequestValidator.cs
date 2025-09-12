using DemoProductApi.Application.Models.Requests;
using FluentValidation;

namespace DemoProductApi.Application.Validation;

public class BundleItemCreateRequestValidator : AbstractValidator<BundleItemCreateRequest>
{
    public BundleItemCreateRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("BundleItemCreateRequest.Quantity must be > 0.");
    }
}