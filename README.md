# IntergalaxyTech API - Prueba Tecnica Lead .NET

API REST en .NET 8 para importar personajes de Rick and Morty y gestionar solicitudes de contratacion.

## Arquitectura elegida

Se implementa Clean Architecture simplificada:

- **Domain**: entidades y reglas de negocio, por ejemplo transiciones validas de `Solicitud`.
- **Application**: casos de uso, DTOs, validaciones e interfaces.
- **Infrastructure**: EF Core, repositorios y cliente externo Rick and Morty.
- **API**: controladores, Swagger, health check y middleware global de errores.

Esta separacion permite mantener la logica fuera de los controladores, facilita pruebas unitarias y cumple inversion de dependencias.

## Requisitos

- .NET 8 SDK
- Docker Desktop opcional

## Ejecutar localmente

```bash
dotnet restore src/IntergalaxyTech.Api/IntergalaxyTech.Api.csproj
dotnet run --project src/IntergalaxyTech.Api/IntergalaxyTech.Api.csproj
```

Swagger:

```txt
http://localhost:5000/swagger
```

Health check:

```txt
GET /health
```

## Ejecutar con Docker

```bash
docker compose up --build
```

Swagger:

```txt
http://localhost:8080/swagger
```

## EF Core Migrations

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/IntergalaxyTech.Infrastructure --startup-project src/IntergalaxyTech.Api
dotnet ef database update --project src/IntergalaxyTech.Infrastructure --startup-project src/IntergalaxyTech.Api
```

La API tambien ejecuta `Database.Migrate()` al iniciar para facilitar la prueba local.

## Endpoints principales

- `POST /api/personajes/importar`
- `GET /api/personajes?page=1&pageSize=20&nombre=Rick&estado=Alive`
- `GET /api/personajes/{id}`
- `POST /api/solicitudes`
- `GET /api/solicitudes?estado=Pendiente&cliente=Juan`
- `GET /api/solicitudes/{id}`
- `PATCH /api/solicitudes/{id}/estado`
- `GET /api/reportes/solicitudes-resumen`
- `GET /health`

## Ejemplos

Importar personajes:

```json
{
  "nombre": "Rick",
  "page": 1,
  "maxPages": 1
}
```

Crear solicitud:

```json
{
  "personajeId": 1,
  "solicitante": "Bryam Garzon",
  "evento": "Comic Con Bogota",
  "fechaEvento": "2026-01-20T10:00:00Z",
  "idExterno": "EVT-001"
}
```

Cambiar estado:

```json
{
  "estado": "EnProceso",
  "motivoRechazo": null
}
```

## Preparacion Azure

| Necesidad | Servicio Azure sugerido |
|---|---|
| Hospedar la API .NET 8 | Azure App Service, porque soporta .NET 8, health checks, slots y escalado simple. |
| Base de datos relacional | Azure SQL Database, por ser administrado, relacional y compatible con EF Core. |
| Almacenar archivos/reportes PDF | Azure Blob Storage, por costo bajo, durabilidad y acceso controlado. |
| Exponer/versionar API a terceros | Azure API Management, por versionamiento, politicas, throttling y seguridad. |
| Tareas programadas o eventos async | Azure Functions o Azure WebJobs; Functions para jobs event-driven o programados con Timer Trigger. |

La cadena de conexion se lee desde `ConnectionStrings__DefaultConnection`, appsettings o variables de entorno. En Docker Compose se inyecta como variable simulando App Service Configuration o Key Vault.

## Migracion Web Forms a .NET 8

Problemas identificados en el codigo legado:

1. Credenciales hardcodeadas en el code-behind.
2. SQL concatenado, vulnerable a SQL Injection.
3. Logica de UI, validacion, acceso a datos y negocio mezcladas.
4. Sin manejo global de errores ni logging.
5. Uso de `DateTime.Now` sin criterio UTC.
6. Estado guardado en `Session`, dificil de escalar en cloud.
7. Sin pruebas unitarias ni contratos claros.

Reescritura equivalente:

```csharp
[HttpPost]
public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request, CancellationToken ct)
{
    var created = await service.CrearAsync(request, ct);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

La validacion queda en FluentValidation, la persistencia en EF Core y los errores en middleware global.

## Liderazgo tecnico

### Plan de migracion gradual

Primero haria discovery del sistema legado, inventario de pantallas, procesos criticos, datos y dependencias. Luego construiria APIs nuevas por bounded context, empezando por modulos de bajo riesgo. Usaria strangler pattern para reemplazar funciones gradualmente, manteniendo pruebas de regresion y monitoreo.

### Operacion paralela legado/nuevo

Usaria convivencia temporal con sincronizacion controlada de datos, feature flags, rutas por modulo y logs comparativos. Para evitar inconsistencias definiria una fuente de verdad por entidad durante la transicion.

### Equipo de 3 desarrolladores

- Dev 1: API/Application y reglas de negocio.
- Dev 2: Infrastructure, EF Core, Rick and Morty client y Docker.
- Dev 3: pruebas, documentacion, CI/CD y hardening cloud.

Flujo Git: ramas cortas por feature, pull requests obligatorios, minimo 1 aprobacion, validacion de build/tests antes de merge a `main`.

## Pruebas

```bash
dotnet test tests/IntergalaxyTech.Tests/IntergalaxyTech.Tests.csproj
```

## Herramientas IA utilizadas

Se utilizo ChatGPT como apoyo para estructurar la solucion, generar boilerplate inicial, README y recomendaciones de arquitectura. El criterio tecnico, validacion y ajustes finales deben ser revisados por el candidato.
