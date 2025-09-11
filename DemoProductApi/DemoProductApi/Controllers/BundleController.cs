using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/bundle")]
public class BundleController(IBundleService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BundleDto>> GetBundle(Guid id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<BundleDto>> CreateBundle(BundleCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetBundle), new { id = created.BundleId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBundle(Guid id, BundleDto dto, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found bundle id.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBundle(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}