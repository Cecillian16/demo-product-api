using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Application.Repositories;
using DemoProductApi.Application.Services;
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
public class LocationServiceTests
{
    private Mock<IGenericRepository<Location>> _repo = null!;
    private LocationService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<Location>>(MockBehavior.Strict);
        _svc = new LocationService(_repo.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsList()
    {
        var list = new List<Location>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Loc A",
                City = "CityX",
                Country = "US"
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
             .ReturnsAsync((Location?)null);
        var loc = await _svc.GetAsync(id);
        loc.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new LocationCreateRequest
        {
            Name = "Warehouse 1",
            Type = "WH",
            Address1 = "123 Road",
            City = "Metro",
            Country = "US"
        };

        _repo.Setup(r => r.AddAsync(It.Is<Location>(l =>
                l.Name == request.Name &&
                l.Type == request.Type &&
                l.City == request.City &&
                l.Country == request.Country &&
                l.Id != Guid.Empty), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);
        created.Id.Should().NotBe(Guid.Empty);
        created.Name.Should().Be(request.Name);
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var entity = new Location { Id = Guid.NewGuid(), Name = "Old" };
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), entity);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var entity = new Location { Id = Guid.NewGuid(), Name = "Old" };
        _repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Location?)null);
        var ok = await _svc.UpdateAsync(entity.Id, entity);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_Success()
    {
        var existing = new Location { Id = Guid.NewGuid(), Name = "Before" };
        var incoming = new Location { Id = existing.Id, Name = "After" };

        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var ok = await _svc.UpdateAsync(existing.Id, incoming);
        ok.Should().BeTrue();
        existing.Name.Should().Be("After");
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Location?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var existing = new Location { Id = Guid.NewGuid(), Name = "ToDelete" };
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