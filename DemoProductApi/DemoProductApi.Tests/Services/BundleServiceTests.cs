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
public class BundleServiceTests
{
    private Mock<IGenericRepository<Bundle>> _repo = null!;
    private BundleService _svc = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IGenericRepository<Bundle>>(MockBehavior.Strict);
        _svc = new BundleService(_repo.Object);
    }

    [Test]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Bundle?)null);
        var dto = await _svc.GetAsync(id);
        dto.Should().BeNull();
        _repo.VerifyAll();
    }

    [Test]
    public async Task CreateAsync_AssignsNewId()
    {
        var request = new BundleCreateRequest
        {
            Name = "Bundle A",
            Description = "Desc",
            Status = (int)Status.Active,
            Items =
            {
                new BundleItemDto { ChildProductItemId = Guid.NewGuid(), Quantity = 2 }
            }
        };

        _repo.Setup(r => r.AddAsync(It.Is<Bundle>(b =>
                b.BundleId != Guid.Empty &&
                b.Name == request.Name &&
                b.Items.Count == 1), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var created = await _svc.CreateAsync(request);

        created.BundleId.Should().NotBe(Guid.Empty);
        created.Name.Should().Be(request.Name);
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_IdMismatch_ReturnsFalse()
    {
        var dto = TestBuilders.NewBundleDto();
        var ok = await _svc.UpdateAsync(Guid.NewGuid(), dto);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_NotFound_ReturnsFalse()
    {
        var dto = TestBuilders.NewBundleDto();
        _repo.Setup(r => r.GetByIdAsync(dto.BundleId, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Bundle?)null);
        var ok = await _svc.UpdateAsync(dto.BundleId, dto);
        ok.Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task UpdateAsync_MergeItems()
    {
        var bundle = TestBuilders.NewBundle();

        var oldChildId = Guid.NewGuid();
        bundle.Items.Add(new BundleItem { BundleId = bundle.BundleId, ChildProductItemId = oldChildId, Quantity = 1 });

        var newChildId = Guid.NewGuid();

        var dto = new BundleDto
        {
            BundleId = bundle.BundleId,
            Name = "Updated",
            Description = "New Desc",
            Status = (int)Status.Active,
            Items =
            {
                new BundleItemDto { ChildProductItemId = newChildId, Quantity = 3 }
            }
        };

        _repo.Setup(r => r.GetByIdAsync(bundle.BundleId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(bundle);
        _repo.Setup(r => r.Update(bundle));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var ok = await _svc.UpdateAsync(bundle.BundleId, dto);

        ok.Should().BeTrue();
        bundle.Items.Should().ContainSingle(i => i.ChildProductItemId == newChildId);
        bundle.Items.Any(i => i.ChildProductItemId == oldChildId).Should().BeFalse();
        _repo.VerifyAll();
    }

    [Test]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Bundle?)null);
        var ok = await _svc.DeleteAsync(id);
        ok.Should().BeFalse();
    }

    [Test]
    public async Task DeleteAsync_Success()
    {
        var bundle = TestBuilders.NewBundle();
        _repo.Setup(r => r.GetByIdAsync(bundle.BundleId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(bundle);
        _repo.Setup(r => r.Remove(bundle));
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        var ok = await _svc.DeleteAsync(bundle.BundleId);
        ok.Should().BeTrue();
        _repo.VerifyAll();
    }
}