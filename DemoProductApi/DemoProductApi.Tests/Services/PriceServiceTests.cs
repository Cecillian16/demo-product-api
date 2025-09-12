using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Services;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemoProductApi.Tests.Services;

[TestFixture]
public class PriceServiceTests
{
    private Mock<IGenericRepository<Price>> _repo = null!;
    private PriceService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<Price>>(MockBehavior.Strict);
        _svc = new PriceService(_repo.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsList()
    {
        var list = new List<Price>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EntityType = PriceEntityType.Product,
                EntityId = Guid.NewGuid(),
                Currency = "USD",
                ListPrice = 100,
                SalePrice = 90,
                ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
            }
        };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(list);
        var result = await _svc.GetAllAsync();
        result.Should().HaveCount(1);
        _repo.VerifyAll();
    }

    [Test]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Price?)null);
        var price = await _svc.GetAsync(id);
        price.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new PriceCreateRequest
        {
            EntityType = PriceEntityType.Item,
            EntityId = Guid.NewGuid(),
            Currency = "EUR",
            ListPrice = 250m,
            SalePrice = 200m,
            ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30))
        };

        _repo.Setup(r => r.AddAsync(It.Is<Price>(p =>
                p.Id != Guid.Empty &&
                p.EntityType == request.EntityType &&
                p.EntityId == request.EntityId &&
                p.Currency == request.Currency &&
                p.ListPrice == request.ListPrice &&
                p.SalePrice == request.SalePrice), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);
        created.Id.Should().NotBe(Guid.Empty);
        created.Currency.Should().Be("EUR");
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var price = new Price { Id = Guid.NewGuid(), Currency = "USD" };
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), price);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var price = new Price { Id = Guid.NewGuid(), Currency = "USD" };
        _repo.Setup(r => r.GetByIdAsync(price.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Price?)null);
        var ok = await _svc.UpdateAsync(price.Id, price);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_Success()
    {
        var existing = new Price
        {
            Id = Guid.NewGuid(),
            EntityType = PriceEntityType.Product,
            EntityId = Guid.NewGuid(),
            Currency = "USD",
            ListPrice = 100m,
            SalePrice = 90m,
            ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-2)),
            ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(5))
        };

        var incoming = new Price
        {
            Id = existing.Id,
            EntityType = PriceEntityType.Item,
            EntityId = Guid.NewGuid(),
            Currency = "GBP",
            ListPrice = 300m,
            SalePrice = 250m,
            ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(60))
        };

        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var ok = await _svc.UpdateAsync(existing.Id, incoming);
        ok.Should().BeTrue();
        existing.Currency.Should().Be("GBP");
        existing.ListPrice.Should().Be(300m);
        existing.EntityType.Should().Be(PriceEntityType.Item);
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Price?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var existing = new Price { Id = Guid.NewGuid(), ListPrice = 10m };
        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Remove(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var ok = await _svc.DeleteAsync(existing.Id);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }
}