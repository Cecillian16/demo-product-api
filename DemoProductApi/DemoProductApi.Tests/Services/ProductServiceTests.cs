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
public class ProductServiceTests
{
    private Mock<IGenericRepository<Product>> _repo = null!;
    private ProductService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<Product>>(MockBehavior.Strict);
        _svc = new ProductService(_repo.Object);
    }

    [Test]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var result = await _svc.GetAsync(Guid.NewGuid());
        result.Should().BeNull();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAsync_Found_ReturnsDto()
    {
        var product = TestBuilders.NewProduct();
        _repo.Setup(r => r.GetByIdAsync(product.ProductId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);
        var result = await _svc.GetAsync(product.ProductId);
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(product.ProductId);
        _repo.VerifyAll();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new ProductCreateRequest
        {
            Name = "Prod A",
            SkuPrefix = "PA",
            Description = "Desc",
            Status = (int)Status.Active,
            VariantOptions =
            {
                new VariantOptionCreateRequest
                {
                    Name = "Color",
                    Values =
                    {
                        new VariantOptionValueCreateRequest { Value = "Red", Code = "R" },
                        new VariantOptionValueCreateRequest { Value = "Blue", Code = "B" }
                    }
                }
            }
        };

        _repo.Setup(r => r.AddAsync(It.Is<Product>(p =>
                p.ProductId != Guid.Empty &&
                p.Name == request.Name &&
                p.VariantOptions.Count == 1 &&
                p.VariantOptions.First().Values.Count == 2), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);
        created.ProductId.Should().NotBe(Guid.Empty);
        created.Name.Should().Be(request.Name);
        created.VariantOptions.Should().HaveCount(1);
        created.VariantOptions.First().Values.Should().HaveCount(2);
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var request = TestBuilders.NewProductRequest();
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), request);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var dto = TestBuilders.NewProductDto();
        var newProd = TestBuilders.NewProductRequest();
        _repo.Setup(r => r.GetByIdAsync(dto.ProductId, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var ok = await _svc.UpdateAsync(dto.ProductId, newProd);
        ok.Should().BeFalse();
        _repo.Verify(r => r.GetByIdAsync(dto.ProductId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_Success()
    {
        var product = TestBuilders.NewProduct();
        var newProd = TestBuilders.NewProductRequest();

        _repo.Setup(r => r.GetByIdAsync(product.ProductId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);
        var replacement = new Product();
        _repo.Setup(r => r.Update(product, It.IsAny<Product>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask).Callback<Product, Product, CancellationToken>((e, u, ct) =>
             {
                 replacement = u;
             });
        var ok = await _svc.UpdateAsync(product.ProductId, newProd);
        ok.Should().BeTrue();
        replacement.Name.Should().Be("Prod DTO");
        replacement.Status.Should().Be(Status.Active);
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var product = TestBuilders.NewProduct();
        _repo.Setup(r => r.GetByIdAsync(product.ProductId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);
        _repo.Setup(r => r.Remove(product));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        var ok = await _svc.DeleteAsync(product.ProductId);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(Array.Empty<Product>());
        var result = await _svc.GetAllAsync();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAllAsync_ReturnsDtos()
    {
        var p1 = TestBuilders.NewProduct();
        var p2 = TestBuilders.NewProduct();
        p1.Name = "A";
        p2.Name = "B";

        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(new List<Product> { p1, p2 });

        var list = await _svc.GetAllAsync();
        list.Should().HaveCount(2);
        list.Select(p => p.ProductId).Should().Contain(new[] { p1.ProductId, p2.ProductId });
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_Succeeds()
    {
        var existing = TestBuilders.NewProduct();
        var newProd = TestBuilders.NewProductRequest();

        var dto = new ProductDto
        {
            ProductId = existing.ProductId,
            Name = "Updated Name",
            SkuPrefix = existing.SkuPrefix,
            Status = Status.Inactive,
            Description = "New Desc",
            VariantOptions = new()
            {
                new VariantOptionDto
                {
                    VariantOptionId = Guid.NewGuid(),
                    Name = "Material",
                    Values =
                    {
                        new VariantOptionValueDto
                        {
                            VariantOptionValueId = Guid.NewGuid(),
                            Value = "Cotton",
                            Code = "COT"
                        }
                    }
                }
            }
        };

        _repo.Setup(r => r.Update(It.IsAny<Product>(), It.IsAny<Product>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.GetByIdAsync(existing.ProductId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        var ok = await _svc.UpdateAsync(existing.ProductId, newProd);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }
}