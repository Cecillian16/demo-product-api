using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/price")]
public class PriceController(IPriceService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Price>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Price>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await service.GetAsync(id, ct);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Price>> Create(PriceCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Price price, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, price, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found price id.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}