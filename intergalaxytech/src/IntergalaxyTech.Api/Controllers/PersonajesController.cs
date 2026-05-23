using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntergalaxyTech.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PersonajesController(PersonajeService service) : ControllerBase
{
    /// <summary>
    ///  Guardar Importacion de personas de Api externa a Base de datos local
    /// </summary>
    /// <param name="request"></param>
    /// <returns> IActionResult </returns>
    [HttpPost]
    public async Task<IActionResult> Importar([FromBody] ImportarPersonajesRequest request, CancellationToken ct)
    {
        try
        {
            var data = await service.ImportarAsync(request, ct);

            if (data.Importados > 0 || data.Actualizados > 0)
            {
                return Ok(data);
            }
            else
            {
                return NotFound("Error 404: No se encontro información para migrar.");
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
    ///  <param name="nombre"></param>
    ///  <param name="estado"></param>
    /// <returns> IActionResult </returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20, string? nombre = null, string? estado = null, CancellationToken ct = default)
    {
        try
        {
            var result = await service.GetPagedAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), nombre, estado, ct);
            if (result.TotalCount > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("Error 404: No existe datos con los filtro seleccionados.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Consulta registro de personaje especifico 
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
                return NotFound("Error 404: No existe datos con los filtro seleccionados.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
