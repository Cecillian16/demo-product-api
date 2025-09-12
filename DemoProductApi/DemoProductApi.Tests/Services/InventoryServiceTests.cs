using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Services;
using DemoProductApi.Domain.Entities;
using DemoProductApi.Tests.Utils;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemoProductApi.Tests.Services;

[TestFixture]
public class InventoryServiceTests
{
    private Mock<IGenericRepository<Inventory>> _repo = null!;
    private InventoryService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<Inventory>>(MockBehavior.Strict);
        _svc = new InventoryService(_repo.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsList()
    {
        var list = new List<Inventory> { TestBuilders.NewInventory() };
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
             .ReturnsAsync((Inventory?)null);
        var inv = await _svc.GetAsync(id);
        inv.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new InventoryCreateRequest
        {
            ProductItemId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            OnHand = 10,
            Reserved = 2,
            InTransit = 1,
            ReorderPoint = 3
        };

        _repo.Setup(r => r.AddAsync(It.Is<Inventory>(i =>
                i.Id != Guid.Empty &&
                i.ProductItemId == request.ProductItemId &&
                i.LocationId == request.LocationId &&
                i.OnHand == request.OnHand &&
                i.Reserved == request.Reserved &&
                i.InTransit == request.InTransit &&
                i.ReorderPoint == request.ReorderPoint
            ), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);

        created.Id.Should().NotBe(Guid.Empty);
        created.ProductItemId.Should().Be(request.ProductItemId);
        created.LocationId.Should().Be(request.LocationId);
        created.OnHand.Should().Be(request.OnHand);
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var inv = TestBuilders.NewInventory();
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), inv);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var inv = TestBuilders.NewInventory();
        _repo.Setup(r => r.GetByIdAsync(inv.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Inventory?)null);
        var ok = await _svc.UpdateAsync(inv.Id, inv);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_Success()
    {
        var existing = TestBuilders.NewInventory();
        var incoming = TestBuilders.NewInventory(existing.Id);
        incoming.OnHand = existing.OnHand + 5;

        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var ok = await _svc.UpdateAsync(existing.Id, incoming);
        ok.Should().BeTrue();
        existing.OnHand.Should().Be(incoming.OnHand);
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Inventory?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var existing = TestBuilders.NewInventory();
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