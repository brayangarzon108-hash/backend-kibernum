using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Application.Services;
using IntergalaxyTech.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IntergalaxyTech.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SolicitudesController(SolicitudService service) : ControllerBase
{
    /// <summary>
    /// Guardar Solicitud
    /// </summary>
    /// <param name="request"></param>
    /// <returns> IActionResult </returns>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request, CancellationToken ct)
    {
        try
        {
            var created = await service.CrearAsync(request, ct);

            if (created.Id > 0)
            {
                return Ok(created);
            }
            else
            {
                return BadRequest(created);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Consulta todos los registros de bases de datos con paginador
    /// </summary>
    /// <param name="page"></param>
    ///  <param name="pageSize"></param>
    ///  <param name="cliente"></param>
    ///  <param name="estado"></param>
    /// <returns> IActionResult </returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20, EstadoSolicitud? estado = null, string? cliente = null, CancellationToken ct = default)
    {
        try
        {
            var result = await service.GetPagedAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), estado, cliente, ct);
            if (result.TotalCount > 0)
            {
                return Ok(result);
            }
            else
            {
                throw new KeyNotFoundException(
                "No existen datos con los filtros seleccionados.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Consulta solicitud de personaje especifico 
    /// </summary>
    /// <param name="id"></param>
    /// <returns> IActionResult </returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        try
        {
            var result = await service.GetByIdAsync(id, ct);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(new
                {
                    success = false,
                    message = "No existen datos con los filtros seleccionados."
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cambiar estado del evento solicitado
    /// </summary>
    /// <param name="id"></param>
    /// <returns> IActionResult </returns>

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] ActualizarEstadoSolicitudRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await service.CambiarEstadoAsync(id, request, ct));

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
