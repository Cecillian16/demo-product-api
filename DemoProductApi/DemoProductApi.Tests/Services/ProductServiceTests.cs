using DemoProductApi.Application.Interfaces.Services;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoProductApi.Tests.Services;

[TestFixture]
public class ProductServiceTests
{
    private Mock<IProductRepository> _repo = null!;
    private ProductService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IProductRepository>(MockBehavior.Strict);
        _svc = new ProductService(_repo.Object);
    }

    [Test]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var result = await _svc.GetAsync(Guid.NewGuid());
        result.Should().BeNull();
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAsync_Found_ReturnsDto()
    {
        var product = TestBuilders.NewProduct();
        _repo.Setup(r => r.GetByIdAsync(product.ProductId, true, It.IsAny<CancellationToken>()))
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
        var dto = TestBuilders.NewProductDto();
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), dto);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var dto = TestBuilders.NewProductDto();
        _repo.Setup(r => r.GetByIdAsync(dto.ProductId, true, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var ok = await _svc.UpdateAsync(dto.ProductId, dto);
        ok.Should().BeFalse();
        _repo.Verify(r => r.GetByIdAsync(dto.ProductId, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_Success()
    {
        var product = TestBuilders.NewProduct();
        var dto = new ProductDto
        {
            ProductId = product.ProductId,
            Name = "Updated",
            SkuPrefix = "UP",
            Description = "New Desc",
            Status = (int)Status.Inactive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            VariantOptions = new()
        };
        _repo.Setup(r => r.GetByIdAsync(product.ProductId, true, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);
        _repo.Setup(r => r.Update(product));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        var ok = await _svc.UpdateAsync(product.ProductId, dto);
        ok.Should().BeTrue();
        product.Name.Should().Be("Updated");
        product.Status.Should().Be(Status.Inactive);
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, true, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var product = TestBuilders.NewProduct();
        _repo.Setup(r => r.GetByIdAsync(product.ProductId, true, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);
        _repo.Setup(r => r.Remove(product));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        var ok = await _svc.DeleteAsync(product.ProductId);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }
}