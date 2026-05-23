using IntergalaxyTech.Domain.Enums;

namespace IntergalaxyTech.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public string? IdExterno { get; set; }
    public int PersonajeId { get; set; }
    public Personaje? Personaje { get; set; }
    public string Solicitante { get; set; } = string.Empty;
    public string Evento { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public void CambiarEstado(EstadoSolicitud nuevoEstado, string? motivo = null)
    {
        if (!EsTransicionValida(Estado, nuevoEstado))
            throw new InvalidOperationException($"Transicion invalida: {Estado} -> {nuevoEstado}");

        if (nuevoEstado == EstadoSolicitud.Rechazada && string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException("El motivo de rechazo es obligatorio.");

        Estado = nuevoEstado;
        MotivoRechazo = nuevoEstado == EstadoSolicitud.Rechazada ? motivo : null;
        FechaActualizacion = DateTime.UtcNow;
    }

    public static bool EsTransicionValida(EstadoSolicitud actual, EstadoSolicitud nueva) =>
        (actual, nueva) switch
        {
            (EstadoSolicitud.Pendiente, EstadoSolicitud.EnProceso) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Aprobada) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Rechazada) => true,
            (EstadoSolicitud.Pendiente, EstadoSolicitud.Rechazada) => true,
            _ => false
        };
}
