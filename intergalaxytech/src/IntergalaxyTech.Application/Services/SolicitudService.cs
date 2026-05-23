using FluentValidation;
using IntergalaxyTech.Application.Abstractions;
using IntergalaxyTech.Application.Common;
using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Domain.Entities;
using IntergalaxyTech.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IntergalaxyTech.Application.Services;

public class SolicitudService
{
    private readonly ISolicitudRepository _repo;
    private readonly IPersonajeRepository _personajes;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CrearSolicitudRequest> _crearValidator;
    private readonly IValidator<ActualizarEstadoSolicitudRequest> _estadoValidator;
    private readonly ILogger<SolicitudService> _logger;

    public SolicitudService(ISolicitudRepository repo, IPersonajeRepository personajes, IUnitOfWork uow,
        IValidator<CrearSolicitudRequest> crearValidator, IValidator<ActualizarEstadoSolicitudRequest> estadoValidator,
        ILogger<SolicitudService> logger)
    {
        _repo = repo; _personajes = personajes; _uow = uow; _crearValidator = crearValidator; _estadoValidator = estadoValidator; _logger = logger;
    }

    /// <summary>
    /// Guardar Solicitud
    /// </summary>
    /// <param name="request"></param>
    /// <returns> IActionResult </returns>
    public async Task<SolicitudDto> CrearAsync(CrearSolicitudRequest request, CancellationToken ct)
    {
        // Validamos que lso registras si cumplan con la validacion estandar
        await _crearValidator.ValidateAndThrowAsync(request, ct);

        // Consultamos si el personsaje existe
        _ = await _personajes.GetByIdAsync(request.PersonajeId, ct) ?? throw new KeyNotFoundException("Personaje no encontrado");

        // Creamos nuevo registro
        var solicitud = new Solicitud
        {
            PersonajeId = request.PersonajeId,
            Solicitante = request.Solicitante.Trim(),
            Evento = request.Evento.Trim(),
            FechaEvento = request.FechaEvento,
            IdExterno = request.IdExterno,
            Estado = EstadoSolicitud.Pendiente,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };
        await _repo.AddAsync(solicitud, ct);
        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Solicitud creada: {SolicitudId}", solicitud.Id);
        return ToDto(solicitud);
    }

    /// <summary>
    /// Cambiar estado del evento solicitado
    /// </summary>
    /// <param name="id"></param>
    /// <returns> IActionResult </returns>
    public async Task<SolicitudDto> CambiarEstadoAsync(int id, ActualizarEstadoSolicitudRequest request, CancellationToken ct)
    {
        // Validamos que el estado enviado sea válido
        await _estadoValidator.ValidateAndThrowAsync(request, ct);

        // Consultamos el id de evento para actualizar
        var solicitud = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Solicitud no encontrada");
        solicitud = CambiarEstado(request.Estado, request.MotivoRechazo, solicitud);
        await _uow.SaveChangesAsync(ct);
        return ToDto(solicitud);
    }

    /// <summary>
    /// Consulta todos los registros de bases de datos con paginador
    /// </summary>
    /// <param name="page"></param>
    ///  <param name="pageSize"></param>
    ///  <param name="cliente"></param>
    ///  <param name="estado"></param>
    /// <returns> IActionResult </returns>
    public async Task<PagedResult<SolicitudDto>> GetPagedAsync(int page, int pageSize, EstadoSolicitud? estado, string? cliente, CancellationToken ct)
    {
        var data = await _repo.GetPagedAsync(page, pageSize, estado, cliente, ct);
        return new PagedResult<SolicitudDto>(data.Items.Select(ToDto).ToList(), data.Page, data.PageSize, data.TotalCount);
    }

    /// <summary>
    /// Consulta registro de evento especifico 
    /// </summary>
    /// <param name="id"></param>
    /// <returns> IActionResult </returns>
    public async Task<SolicitudDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Solicitud no encontrada");
        return ToDto(s);
    }

    public async Task<ResumenSolicitudesDto> ResumenAsync(CancellationToken ct)
    {
        var conteos = await _repo.CountByEstadoAsync(ct);
        return new ResumenSolicitudesDto(conteos.ToDictionary(x => x.Key.ToString(), x => x.Value), await _repo.CountAsync(ct), await _repo.GetPersonajeMasSolicitadoAsync(ct));
    }

    /// <summary>
    /// Realizamos la validacion de cambio de estado
    /// </summary>
    /// <param name="nuevoEstado"></param>
    /// <returns> Solicitud </returns>
    private Solicitud CambiarEstado(EstadoSolicitud nuevoEstado, string? motivo = null, Solicitud? data = null)
    {
        if (!EsTransicionValida(data.Estado, nuevoEstado))
            throw new InvalidOperationException($"Transicion invalida: {data.Estado} -> {nuevoEstado}");

        if (nuevoEstado == EstadoSolicitud.Rechazada && string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException("El motivo de rechazo es obligatorio.");

        data.Estado = nuevoEstado;
        data.MotivoRechazo = nuevoEstado == EstadoSolicitud.Rechazada ? motivo : null;
        data.FechaActualizacion = DateTime.UtcNow;

        return data;
    }

    /// <summary>
    /// Validamos que si se pueda hacer la conversion de estado
    /// </summary>
    /// <param name="actual"></param>
    /// <returns> Solicitud </returns>
    private bool EsTransicionValida(EstadoSolicitud actual, EstadoSolicitud nueva) =>
        (actual, nueva) switch
        {
            (EstadoSolicitud.Pendiente, EstadoSolicitud.EnProceso) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Aprobada) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Rechazada) => true,
            (EstadoSolicitud.Pendiente, EstadoSolicitud.Rechazada) => true,
            _ => false
        };

    private static SolicitudDto ToDto(Solicitud s) => new(s.Id, s.IdExterno, s.PersonajeId, s.Personaje?.Nombre, s.Solicitante, s.Evento, s.FechaEvento, s.Estado, s.MotivoRechazo, s.FechaCreacion, s.FechaActualizacion);
}
