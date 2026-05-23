using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Application.Services;
using IntergalaxyTech.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IntergalaxyTech.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SolicitudesController(SolicitudService service) : ControllerBase
{    
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request, CancellationToken ct)
    {
        var created = await service.CrearAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] EstadoSolicitud? estado = null, [FromQuery] string? cliente = null, CancellationToken ct = default)
        => Ok(await service.GetPagedAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), estado, cliente, ct));

    [HttpGet("{id:int}")]
    public Task<SolicitudDto> GetById(int id, CancellationToken ct) => service.GetByIdAsync(id, ct);

    [HttpPatch("{id:int}")]
    public Task<SolicitudDto> CambiarEstado(int id, [FromBody] ActualizarEstadoSolicitudRequest request, CancellationToken ct) => service.CambiarEstadoAsync(id, request, ct);
}
