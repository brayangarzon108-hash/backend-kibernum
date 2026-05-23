using IntergalaxyTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntergalaxyTech.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Personaje> Personajes => Set<Personaje>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personaje>(e =>
        {
            e.HasIndex(x => x.ExternalId).IsUnique();
            e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            e.Property(x => x.Estado).HasMaxLength(30);
            e.Property(x => x.Especie).HasMaxLength(80);
            e.Property(x => x.Genero).HasMaxLength(50);
            e.Property(x => x.Origen).HasMaxLength(150);
            e.Property(x => x.Ubicacion).HasMaxLength(150);
            e.Property(x => x.ImagenUrl).HasMaxLength(500);
        });
        modelBuilder.Entity<Solicitud>(e =>
        {
            e.Property(x => x.Solicitante).HasMaxLength(150).IsRequired();
            e.Property(x => x.Evento).HasMaxLength(200).IsRequired();
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.MotivoRechazo).HasMaxLength(500);
            e.HasOne(x => x.Personaje).WithMany(x => x.Solicitudes).HasForeignKey(x => x.PersonajeId);
        });
    }
}
