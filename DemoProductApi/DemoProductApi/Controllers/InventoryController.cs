using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inventory>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Inventory>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await service.GetAsync(id, ct);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Inventory>> Create(InventoryCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Inventory inventory, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, inventory, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found inventory id.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}