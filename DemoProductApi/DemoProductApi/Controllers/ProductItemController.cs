using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/productitem")]
public class ProductItemController(IProductItemService service) : ControllerBase
{
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
    public async Task<IActionResult> UpdateProductItem(Guid id, ProductItemDto dto, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found product item id or duplicate variant options.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductItem(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
