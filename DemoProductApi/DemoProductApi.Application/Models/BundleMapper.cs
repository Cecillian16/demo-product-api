using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models;

public static class BundleMapper
{
    public static Bundle ToEntity(BundleCreateRequest request, Guid id)
    {
        var now = DateTimeOffset.UtcNow;
        var bundleId = id != Guid.Empty ? id : Guid.NewGuid();

        var bundle = new Bundle
        {
            BundleId = bundleId,
            Name = request.Name,
            Description = request.Description,
            Status = (Status)request.Status,
            CreatedAt = now,
            UpdatedAt = now,
            Items = request.Items.Select(it =>
            {
                return new BundleItem
                {
                    Id = Guid.NewGuid(),
                    BundleId = bundleId,
                    ChildProductItemId = it.ChildProductItemId ?? Guid.Empty,
                    Quantity = it.Quantity,
                };
            }).ToList()
        };

        return bundle;
    }

    public static BundleDto ToDto(Bundle entity)
    {
        return new BundleDto
        {
            BundleId = entity.BundleId,
            Name = entity.Name,
            Description = entity.Description,
            Status = (int)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Items = entity.Items.Select(i => new BundleItemDto
            {
                ChildProductItemId = i.ChildProductItemId,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}