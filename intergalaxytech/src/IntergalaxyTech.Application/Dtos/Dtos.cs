using IntergalaxyTech.Domain.Enums;

namespace IntergalaxyTech.Application.Dtos;

public record PersonajeDto(int Id, int ExternalId, string Nombre, string Estado, string Especie, string Genero, string Origen, string Ubicacion, string ImagenUrl, DateTime FechaImport);
public record ImportarPersonajesRequest(string? Nombre, int Page = 1, int MaxPages = 1);
public record ImportarPersonajesResponse(int Importados, int Actualizados);
public record CrearSolicitudRequest(int PersonajeId, string Solicitante, string Evento, DateTime FechaEvento, string? IdExterno);
public record SolicitudDto(int Id, string? IdExterno, int PersonajeId, string? Personaje, string Solicitante, string Evento, DateTime FechaEvento, EstadoSolicitud Estado, string? MotivoRechazo, DateTime FechaCreacion, DateTime FechaActualizacion);
public record ActualizarEstadoSolicitudRequest(EstadoSolicitud Estado, string? MotivoRechazo);
public record ResumenSolicitudesDto(Dictionary<string, int> TotalesPorEstado, int TotalSolicitudes, string? PersonajeMasSolicitado);
