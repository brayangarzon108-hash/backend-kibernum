using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntergalaxyTech.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReportesController(SolicitudService service) : ControllerBase
{
    [HttpGet]
    public Task<ResumenSolicitudesDto> Resumen(CancellationToken ct) => service.ResumenAsync(ct);
}
