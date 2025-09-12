using DemoProductApi.Application.Models.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DemoProductApi.Validators;

public class BundleCreateRequestValidator : AbstractValidator<BundleCreateRequest>
{
    public BundleCreateRequestValidator(IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleForEach(r => r.Items).ChildRules(child =>
        {
            child.RuleFor(i => i.ChildProductItemId)
                 .NotEmpty().WithMessage("ChildProductItemId is required.");
            child.RuleFor(i => i.Quantity)
                 .GreaterThan(0m).WithMessage("Quantity must be greater than zero.");
        });

        // Custom rule to prevent self-reference during update (PUT)
        RuleFor(r => r).Custom((req, context) =>
        {
            var routeValues = httpContextAccessor.HttpContext?.Request.RouteValues;
            if (routeValues is null) return;

            if (routeValues.TryGetValue("id", out var idObj) &&
                Guid.TryParse(idObj?.ToString(), out var routeId) &&
                routeId != Guid.Empty)
            {
                if (req.Items.Any(i => i.ChildProductItemId == routeId))
                {
                    context.AddFailure("Items", "A bundle cannot include itself as a child.");
                }
            }
        });
    }
}