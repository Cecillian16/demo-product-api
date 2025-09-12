using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/productitem")]
public class ProductItemController(IProductItemService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductItemDto>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductItemDto>> GetProductItem(Guid id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductItemDto>> CreateProductItem(ProductItemCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetProductItem), new { id = created.ProductItemId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProductItem(Guid id, ProductItemCreateRequest request, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductItem(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
