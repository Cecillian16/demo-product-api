using System;
using DemoProductApi.Application.Models;
using DemoProductApi.Infrastructure;
using DemoProductApi.Validators;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DemoProductApi.Tests.Validators;

[TestFixture]
public class ProductDtoValidatorTests
{
    private ProductDtoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        // Use in-memory DB; ProductDtoValidator does not currently use DB in logic,
        // but constructor requires AppDbContext.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        _validator = new ProductDtoValidator(db);
    }

    [Test]
    public void Validate_ValidDto_Passes()
    {
        var dto = new ProductDto
        {
            Name = "Test Product",
            SkuPrefix = "TP_1",
            Status = Domain.Status.Active,
            VariantOptions =
            {
                new VariantOptionDto
                {
                    Name = "Color",
                    Values = { new VariantOptionValueDto { Value = "Red", Code = "R" } }
                }
            }
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_InvalidSkuPrefix_Fails()
    {
        var dto = new ProductDto
        {
            Name = "P",
            SkuPrefix = "bad space",
            Status = Domain.Status.Active,
            VariantOptions =
            {
                new VariantOptionDto { Name = "V", Values = { new VariantOptionValueDto { Value = "v" } } }
            }
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SkuPrefix");
    }

    [Test]
    public void Validate_DuplicateVariantOptionNames_Fails()
    {
        var dto = new ProductDto
        {
            Name = "P2",
            SkuPrefix = "P2",
            Status = Domain.Status.Active,
            VariantOptions =
            {
                new VariantOptionDto { Name = "Size", Values = { new VariantOptionValueDto { Value = "S" } } },
                new VariantOptionDto { Name = "size", Values = { new VariantOptionValueDto { Value = "M" } } }
            }
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage != null && e.ErrorMessage.Contains("Duplicate VariantOption Name"));
    }
}
