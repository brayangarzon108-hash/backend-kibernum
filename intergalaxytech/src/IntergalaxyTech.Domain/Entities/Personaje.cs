namespace IntergalaxyTech.Domain.Entities;

public class Personaje
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public DateTime FechaImport { get; set; } = DateTime.UtcNow;
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
