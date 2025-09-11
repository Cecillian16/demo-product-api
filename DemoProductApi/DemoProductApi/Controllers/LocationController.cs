using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models.Requests;
using DemoProductApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/location")]
public class LocationController(ILocationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Location>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await service.GetAsync(id, ct);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Location>> Create(LocationCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Location location, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, location, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found location id.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}