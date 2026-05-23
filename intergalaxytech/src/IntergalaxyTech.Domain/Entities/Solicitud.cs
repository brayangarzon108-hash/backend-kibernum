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

}
