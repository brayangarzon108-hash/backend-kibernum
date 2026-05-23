using FluentValidation;
using IntergalaxyTech.Application.Dtos;
using IntergalaxyTech.Domain.Enums;

namespace IntergalaxyTech.Application.Validators;

public class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudRequestValidator()
    {
        RuleFor(x => x.PersonajeId).GreaterThan(0);
        RuleFor(x => x.Solicitante).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Evento).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FechaEvento).GreaterThan(DateTime.UtcNow.AddDays(-1));
    }
}

public class ActualizarEstadoSolicitudRequestValidator : AbstractValidator<ActualizarEstadoSolicitudRequest>
{
    public ActualizarEstadoSolicitudRequestValidator()
    {
        RuleFor(x => x.Estado).IsInEnum();
        RuleFor(x => x.MotivoRechazo)
            .NotEmpty()
            .When(x => x.Estado == EstadoSolicitud.Rechazada);
    }
}
