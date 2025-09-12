using DemoProductApi.Application.Interfaces.Services;
using DemoProductApi.Application.Models;
using DemoProductApi.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DemoProductApi.Controllers;

[ApiController]
[Route("api/bundle")]
public class BundleController(IBundleService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BundleDto>> GetAllBundleAsync(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}", Name = "GetBundleById")]
    public async Task<ActionResult<BundleDto>> GetBundleByIdAsync(Guid id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<BundleDto>> CreateAsync(BundleCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtRoute("GetBundleById", new { id = created.BundleId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBundleAsync(Guid id, BundleCreateRequest request, CancellationToken ct)
    {
        var ok = await service.UpdateAsync(id, request, ct);
        return ok ? NoContent() : BadRequest("Invalid or not found bundle id.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBundleAsync(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}