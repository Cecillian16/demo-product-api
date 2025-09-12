using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Services;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Tests.Utils;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoProductApi.Tests.Services;

[TestFixture]
public class ProductItemServiceTests
{
    private Mock<IGenericRepository<ProductItem>> _repo = null!;
    private ProductItemService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<ProductItem>>(MockBehavior.Strict);
        _svc = new ProductItemService(_repo.Object);
    }

    [Test]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((ProductItem?)null);
        var dto = await _svc.GetAsync(Guid.NewGuid());
        dto.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new ProductItemCreateRequest
        {
            ProductId = Guid.NewGuid(),
            Sku = "SKU-NEW",
            Status = (int)Status.Active,
            Weight = 1.2m,
            Volume = 0.8m,
            VariantValues =
            {
                new ProductItemVariantValueCreateRequest
                {
                    VariantOptionId = Guid.NewGuid(),
                    VariantOptionValueId = Guid.NewGuid()
                }
            }
        };

        _repo.Setup(r => r.AddAsync(It.Is<ProductItem>(pi =>
                pi.ProductItemId != Guid.Empty &&
                pi.ProductId == request.ProductId &&
                pi.Sku == request.Sku &&
                pi.VariantValues.Count == 1), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);
        created.ProductItemId.Should().NotBe(Guid.Empty);
        created.Sku.Should().Be(request.Sku);
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var request = TestBuilders.NewProductItemRequest();
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), request);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var request = TestBuilders.NewProductItemRequest();
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((ProductItem?)null);
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), request);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_DuplicateVariant_ReturnsFalse()
    {
        var existing = TestBuilders.NewProductItem();
        _repo.Setup(r => r.GetByIdAsync(existing.ProductItemId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        var dup = Guid.NewGuid();
        var request = new ProductItemCreateRequest
        {
            ProductId = existing.ProductId,
            Sku = "SKU-X",
            Status = (int)Status.Active,
            Weight = 1,
            Volume = 1,
            VariantValues = new()
            {
                new ProductItemVariantValueCreateRequest { VariantOptionId = dup, VariantOptionValueId = Guid.NewGuid() },
                new ProductItemVariantValueCreateRequest { VariantOptionId = dup, VariantOptionValueId = Guid.NewGuid() }
            }
        };

        var ok = await _svc.UpdateAsync(existing.ProductItemId, request);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_AddsAndRemovesVariants()
    {
        var existing = TestBuilders.NewProductItem();
        var existingOptionId = Guid.NewGuid();
        existing.VariantValues.Add(new ProductItemVariantValue
        {
            ProductItemId = existing.ProductItemId,
            VariantOptionId = existingOptionId,
            VariantOptionValueId = Guid.NewGuid()
        });

        _repo.Setup(r => r.GetByIdAsync(existing.ProductItemId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var request = new ProductItemCreateRequest
        {
            ProductId = existing.ProductId,
            Sku = "SKU-UP",
            Status = (int)Status.Active,
            Weight = 2,
            Volume = 2,
            VariantValues = new()
            {
                new ProductItemVariantValueCreateRequest
                {
                    VariantOptionId = Guid.NewGuid(),
                    VariantOptionValueId = Guid.NewGuid()
                },
                new ProductItemVariantValueCreateRequest
                {
                    VariantOptionId = Guid.NewGuid(),
                    VariantOptionValueId = Guid.NewGuid()
                }
            }
        };

        var ok = await _svc.UpdateAsync(existing.ProductItemId, request);
        ok.Should().BeTrue();
        existing.VariantValues.Should().HaveCount(2);
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((ProductItem?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var existing = TestBuilders.NewProductItem();
        _repo.Setup(r => r.GetByIdAsync(existing.ProductItemId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Remove(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        var ok = await _svc.DeleteAsync(existing.ProductItemId);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(Array.Empty<ProductItem>());
        var list = await _svc.GetAllAsync();
        list.Should().NotBeNull();
        list.Should().BeEmpty();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var item1 = TestBuilders.NewProductItem();
        var item2 = TestBuilders.NewProductItem();
        item1.Sku = "SKU-1";
        item2.Sku = "SKU-2";

        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(new List<ProductItem> { item1, item2 });

        var list = await _svc.GetAllAsync();

        list.Should().HaveCount(2);
        list.Select(x => x.Sku).Should().BeEquivalentTo(new[] { "SKU-1", "SKU-2" });
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAsync_Found_ReturnsDto()
    {
        var entity = TestBuilders.NewProductItem();
        entity.Sku = "FOUND-SKU";
        _repo.Setup(r => r.GetByIdAsync(entity.ProductItemId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(entity);

        var dto = await _svc.GetAsync(entity.ProductItemId);

        dto.Should().NotBeNull();
        dto!.ProductItemId.Should().Be(entity.ProductItemId);
        dto.Sku.Should().Be("FOUND-SKU");
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_EmptyId_ReturnsFalse()
    {
        var request = TestBuilders.NewProductItemRequest();
        var ok = await _svc.UpdateAsync(Guid.Empty, request);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_UpdateExistingVariantValue()
    {
        // Existing entity with a variant value
        var existing = TestBuilders.NewProductItem();
        var optionId = Guid.NewGuid();
        var originalValueId = Guid.NewGuid();
        existing.VariantValues.Add(new ProductItemVariantValue
        {
            ProductItemId = existing.ProductItemId,
            VariantOptionId = optionId,
            VariantOptionValueId = originalValueId
        });

        _repo.Setup(r => r.GetByIdAsync(existing.ProductItemId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var newValueId = Guid.NewGuid();
        var request = new ProductItemCreateRequest
        {
            ProductId = existing.ProductId,
            Sku = "SKU-CHANGED",
            Status = (int)Status.Active,
            Weight = 5,
            Volume = 3,
            VariantValues = new()
            {
                new ProductItemVariantValueCreateRequest
                {
                    VariantOptionId = optionId,
                    VariantOptionValueId = newValueId
                }
            }
        };

        var ok = await _svc.UpdateAsync(existing.ProductItemId, request);
        ok.Should().BeTrue();
        existing.VariantValues.Should().HaveCount(1);
        existing.VariantValues.Single().VariantOptionValueId.Should().Be(newValueId);
        _repo.VerifyAll();
    }
}