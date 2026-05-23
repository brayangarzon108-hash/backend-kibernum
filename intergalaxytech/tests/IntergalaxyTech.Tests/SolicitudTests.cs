using IntergalaxyTech.Domain.Entities;
using IntergalaxyTech.Domain.Enums;
using Xunit;

namespace IntergalaxyTech.Tests;

public class SolicitudTests
{
    [Fact]
    public void Permite_transicion_de_pendiente_a_en_proceso()
    {
        var solicitud = new Solicitud { Estado = EstadoSolicitud.Pendiente };
        CambiarEstado(EstadoSolicitud.EnProceso, null, solicitud);
        Assert.Equal(EstadoSolicitud.EnProceso, solicitud.Estado);
    }

    [Fact]
    public void Rechazo_requiere_motivo()
    {
        var solicitud = new Solicitud { Estado = EstadoSolicitud.Pendiente };
        Assert.Throws<InvalidOperationException>(() => CambiarEstado(EstadoSolicitud.Rechazada, null, solicitud));
    }

    [Fact]
    public void No_permite_aprobar_desde_pendiente()
    {
        var solicitud = new Solicitud { Estado = EstadoSolicitud.Pendiente };
        Assert.Throws<InvalidOperationException>(() => CambiarEstado(EstadoSolicitud.Aprobada, null, solicitud));
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
}
