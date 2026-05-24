using IntergalaxyTech.Application.Abstractions;
using IntergalaxyTech.Application.Common;
using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IntergalaxyTech.Application.Services;

public class PersonajeService
{
    private readonly IRickAndMortyClient _client;
    private readonly IPersonajeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PersonajeService> _logger;

    public PersonajeService(IRickAndMortyClient client, IPersonajeRepository repo, IUnitOfWork uow, ILogger<PersonajeService> logger)
    {
        _client = client; _repo = repo; _uow = uow; _logger = logger;
    }

    /// <summary>
    ///  Guardar Importacion de personas de Api externa a Base de datos local
    /// </summary>
    /// <param name="request"></param>
    /// <returns> IActionResult </returns>
    public async Task<ImportarPersonajesResponse> ImportarAsync(ImportarPersonajesRequest request, CancellationToken ct)
    {
        var importados = 0; var actualizados = 0;
        for (var page = request.Page; page <= request.MaxPages; page++)
        {
            var response = await _client.GetCharactersAsync(page, request.Nombre, ct);

            if (response.Info.Count > 0 && response.Results.Count() > 0)
            {
                foreach (var c in response.Results)
                {
                    var personaje = await _repo.GetByExternalIdAsync(c.Id, ct);
                    if (personaje is null)
                    {
                        await _repo.AddAsync(new Personaje
                        {
                            ExternalId = c.Id,
                            Nombre = c.Name,
                            Estado = c.Status,
                            Especie = c.Species,
                            Genero = c.Gender,
                            Origen = c.Origin.Name,
                            Ubicacion = c.Location.Name,
                            ImagenUrl = c.Image,
                            FechaImport = DateTime.UtcNow
                        }, ct);
                        importados++;
                    }
                    else
                    {
                        personaje.Nombre = c.Name; personaje.Estado = c.Status; personaje.Especie = c.Species;
                        personaje.Genero = c.Gender; personaje.Origen = c.Origin.Name; personaje.Ubicacion = c.Location.Name;
                        personaje.ImagenUrl = c.Image; actualizados++;
                    }
                }
            }
            else
            {
                return new ImportarPersonajesResponse(0, 0);
            }
            if (page >= response.Info.Pages) break;
        }
        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Importacion finalizada. Importados: {Importados}, Actualizados: {Actualizados}", importados, actualizados);
        return new ImportarPersonajesResponse(importados, actualizados);
    }

    /// <summary>
    /// Consulta todos los registros de bases de datos con paginador
    /// </summary>
    /// <param name="page"></param>
    ///  <param name="pageSize"></param>
    ///  <param name="nombre"></param>
    ///  <param name="estado"></param>
    /// <returns> IActionResult </returns>
    public async Task<PagedResult<PersonajeDto>> GetPagedAsync(int page, int pageSize, string? nombre, string? estado, CancellationToken ct)
    {
        var data = await _repo.GetPagedAsync(page, pageSize, nombre, estado, ct);
        return new PagedResult<PersonajeDto>(data.Items.Select(ToDto).ToList(), data.Page, data.PageSize, data.TotalCount);
    }

    /// <summary>
    /// Consulta registro de evento especifico 
    /// </summary>
    /// <param name="id"></param>
    /// <returns> IActionResult </returns>
    public async Task<PersonajeDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Personaje no encontrado");
        return ToDto(p);
    }

    private static PersonajeDto ToDto(Personaje p) => new(p.Id, p.ExternalId, p.Nombre, p.Estado, p.Especie, p.Genero, p.Origen, p.Ubicacion, p.ImagenUrl, p.FechaImport);
}
