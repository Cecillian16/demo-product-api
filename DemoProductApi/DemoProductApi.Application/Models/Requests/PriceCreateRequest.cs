using DemoProductApi.Domain;
using DemoProductApi.Domain.Entities;

namespace DemoProductApi.Application.Models.Requests;

public class PriceCreateRequest
{
    public PriceEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ListPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}