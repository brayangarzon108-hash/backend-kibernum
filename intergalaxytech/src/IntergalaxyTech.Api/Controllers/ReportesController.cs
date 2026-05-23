using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IntergalaxyTech.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReportesController(SolicitudService service) : ControllerBase
{
    /// <summary>
    /// Consulta de reportes solicitados
    /// </summary>
    /// <param name="ct"></param>
    /// <returns> IActionResult </returns>
    [HttpGet]
    public async Task<IActionResult> Resumen(CancellationToken ct)
    {
        try
        {
            var result = await service.ResumenAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
