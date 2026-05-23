using IntergalaxyTech.Application.Abstractions;
using IntergalaxyTech.Application.Common;
using IntergalaxyTech.Domain.Entities;
using IntergalaxyTech.Domain.Enums;
using IntergalaxyTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntergalaxyTech.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork { public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct); }

public class PersonajeRepository(AppDbContext db) : IPersonajeRepository
{
    public Task AddAsync(Personaje personaje, CancellationToken ct) => db.Personajes.AddAsync(personaje, ct).AsTask();
    public Task<Personaje?> GetByExternalIdAsync(int externalId, CancellationToken ct) => db.Personajes.FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);
    public Task<Personaje?> GetByIdAsync(int id, CancellationToken ct) => db.Personajes.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<PagedResult<Personaje>> GetPagedAsync(int page, int pageSize, string? nombre, string? estado, CancellationToken ct)
    {
        var q = db.Personajes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(nombre)) q = q.Where(x => x.Nombre.Contains(nombre));
        if (!string.IsNullOrWhiteSpace(estado)) q = q.Where(x => x.Estado == estado);
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Nombre).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Personaje>(items, page, pageSize, total);
    }
}

public class SolicitudRepository(AppDbContext db) : ISolicitudRepository
{
    public Task AddAsync(Solicitud solicitud, CancellationToken ct) => db.Solicitudes.AddAsync(solicitud, ct).AsTask();
    public Task<Solicitud?> GetByIdAsync(int id, CancellationToken ct) => db.Solicitudes.Include(x => x.Personaje).FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<PagedResult<Solicitud>> GetPagedAsync(int page, int pageSize, EstadoSolicitud? estado, string? cliente, CancellationToken ct)
    {
        var q = db.Solicitudes.Include(x => x.Personaje).AsNoTracking().AsQueryable();
        if (estado.HasValue) q = q.Where(x => x.Estado == estado.Value);
        if (!string.IsNullOrWhiteSpace(cliente)) q = q.Where(x => x.Solicitante.Contains(cliente));
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.FechaCreacion).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Solicitud>(items, page, pageSize, total);
    }
    public async Task<Dictionary<EstadoSolicitud, int>> CountByEstadoAsync(CancellationToken ct) => await db.Solicitudes.GroupBy(x => x.Estado).ToDictionaryAsync(x => x.Key, x => x.Count(), ct);
    public Task<int> CountAsync(CancellationToken ct) => db.Solicitudes.CountAsync(ct);
    public Task<string?> GetPersonajeMasSolicitadoAsync(CancellationToken ct) => db.Solicitudes.Include(x => x.Personaje).GroupBy(x => x.Personaje!.Nombre).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefaultAsync(ct);
}
