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
        solicitud.CambiarEstado(EstadoSolicitud.EnProceso);
        Assert.Equal(EstadoSolicitud.EnProceso, solicitud.Estado);
    }

    [Fact]
    public void Rechazo_requiere_motivo()
    {
        var solicitud = new Solicitud { Estado = EstadoSolicitud.Pendiente };
        Assert.Throws<InvalidOperationException>(() => solicitud.CambiarEstado(EstadoSolicitud.Rechazada));
    }

    [Fact]
    public void No_permite_aprobar_desde_pendiente()
    {
        var solicitud = new Solicitud { Estado = EstadoSolicitud.Pendiente };
        Assert.Throws<InvalidOperationException>(() => solicitud.CambiarEstado(EstadoSolicitud.Aprobada));
    }
}
