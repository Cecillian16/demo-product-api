using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService service) : ControllerBase
{
    // GET: api/product/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    // POST: api/product
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetProduct), new { id = created.ProductId }, created);
    }

    // PUT: api/product/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductDto dto, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found product id.");
    }

    // DELETE: api/product/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}