using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService service) : ControllerBase
{
    // GET: api/product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductsAsync(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    // GET: api/product/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    // POST: api/product
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProductAsync(ProductCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetProductByIdAsync), new { id = created.ProductId }, created);
    }

    // PUT: api/product/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProductAsync(Guid id, ProductCreateRequest request, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, request, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found product id.");
    }

    // DELETE: api/product/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductAsync(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}