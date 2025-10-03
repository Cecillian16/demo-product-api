using System;
using DemoProductApi.Application.Models;
using DemoProductApi.Infrastructure;
using DemoProductApi.Validators;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace DemoProductApi.Tests.Validators;

[TestFixture]
public class ProductItemDtoValidatorTests
{
    private ProductItemDtoValidator _validator = null!;
    private AppDbContext _db = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        // Seed a variant option and value
    var option = new Domain.Entities.VariantOption { VariantOptionId = Guid.NewGuid(), Name = "Color", ProductId = Guid.NewGuid() };
    var value = new Domain.Entities.VariantOptionValue { VariantOptionValueId = Guid.NewGuid(), VariantOptionId = option.VariantOptionId, Value = "Red" };
        _db.VariantOptions.Add(option);
        _db.VariantOptionValues.Add(value);
        _db.SaveChanges();

        _validator = new ProductItemDtoValidator(_db);
    }

    [Test]
    public async Task Validate_StatusInt_ProducesValidationError()
    {
        var dto = new ProductItemDto
        {
            Sku = "SKU1",
            ProductId = Guid.NewGuid(),
            Status = (int)Domain.Status.Active,
            VariantValues =
            {
                new ProductItemVariantValueDto { VariantOptionId = _db.VariantOptions.First().VariantOptionId, VariantOptionValueId = _db.VariantOptionValues.First().VariantOptionValueId }
            }
        };

        var result = await _validator.ValidateAsync(dto);
        // The validator uses IsInEnum on an int-typed property which causes a validation error.
        result.IsValid.Should().BeFalse();
        result.Errors.Select(e => e.PropertyName).Should().Contain("Status");
    }

    [Test]
    public async Task Validate_NonExistentOption_Fails()
    {
        var dto = new ProductItemDto
        {
            Sku = "SKU2",
            ProductId = Guid.NewGuid(),
            Status = (int)Domain.Status.Active,
            VariantValues =
            {
                new ProductItemVariantValueDto { VariantOptionId = Guid.NewGuid(), VariantOptionValueId = Guid.NewGuid() }
            }
        };

        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage != null && e.ErrorMessage.Contains("do not exist"));
    }
}
