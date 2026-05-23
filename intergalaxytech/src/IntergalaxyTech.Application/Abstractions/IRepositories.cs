using IntergalaxyTech.Application.Common;
using IntergalaxyTech.Domain.Entities;
using IntergalaxyTech.Domain.Enums;

namespace IntergalaxyTech.Application.Abstractions;

public interface IPersonajeRepository
{
    Task<Personaje?> GetByIdAsync(int id, CancellationToken ct);
    Task<Personaje?> GetByExternalIdAsync(int externalId, CancellationToken ct);
    Task AddAsync(Personaje personaje, CancellationToken ct);
    Task<PagedResult<Personaje>> GetPagedAsync(int page, int pageSize, string? nombre, string? estado, CancellationToken ct);
}

public interface ISolicitudRepository
{
    Task<Solicitud?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Solicitud solicitud, CancellationToken ct);
    Task<PagedResult<Solicitud>> GetPagedAsync(int page, int pageSize, EstadoSolicitud? estado, string? cliente, CancellationToken ct);
    Task<Dictionary<EstadoSolicitud, int>> CountByEstadoAsync(CancellationToken ct);
    Task<string?> GetPersonajeMasSolicitadoAsync(CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
}

public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken ct); }
