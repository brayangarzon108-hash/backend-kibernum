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

    public async Task<SolicitudDto> CrearAsync(CrearSolicitudRequest request, CancellationToken ct)
    {
        await _crearValidator.ValidateAndThrowAsync(request, ct);
        _ = await _personajes.GetByIdAsync(request.PersonajeId, ct) ?? throw new KeyNotFoundException("Personaje no encontrado");
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

    public async Task<SolicitudDto> CambiarEstadoAsync(int id, ActualizarEstadoSolicitudRequest request, CancellationToken ct)
    {
        await _estadoValidator.ValidateAndThrowAsync(request, ct);
        var solicitud = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Solicitud no encontrada");
        solicitud.CambiarEstado(request.Estado, request.MotivoRechazo);
        await _uow.SaveChangesAsync(ct);
        return ToDto(solicitud);
    }

    public async Task<PagedResult<SolicitudDto>> GetPagedAsync(int page, int pageSize, EstadoSolicitud? estado, string? cliente, CancellationToken ct)
    {
        var data = await _repo.GetPagedAsync(page, pageSize, estado, cliente, ct);
        return new PagedResult<SolicitudDto>(data.Items.Select(ToDto).ToList(), data.Page, data.PageSize, data.TotalCount);
    }

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

    private static SolicitudDto ToDto(Solicitud s) => new(s.Id, s.IdExterno, s.PersonajeId, s.Personaje?.Nombre, s.Solicitante, s.Evento, s.FechaEvento, s.Estado, s.MotivoRechazo, s.FechaCreacion, s.FechaActualizacion);
}
