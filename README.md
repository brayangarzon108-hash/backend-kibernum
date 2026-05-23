# IntergalaxyTech API - Prueba Tecnica Lead .NET

API REST en .NET 8 para importar personajes de Rick and Morty y gestionar solicitudes de evento.

## Arquitectura elegida

Se implementa Clean Architecture simplificada:

- **Domain**: entidades y enum.
- **Application**: lógica de negocio, DTOs, validaciones e interfaces.
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
http://localhost:5000/swagger (El puerto puede cambiar de acuerdo al equipo local utilizado)
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
http://localhost:8080/swagger (El puerto puede cambiar de acuerdo al equipo local utilizado)
```

## EF Core Migrations

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/IntergalaxyTech.Infrastructure --startup-project src/IntergalaxyTech.Api
dotnet ef database update --project src/IntergalaxyTech.Infrastructure --startup-project src/IntergalaxyTech.Api
```

La API tambien ejecuta `Database.Migrate()` al iniciar para facilitar la prueba local.

## Endpoints principales

- `POST /api/personajes/Importar`. Guardar Importacion de personas de Api externa a Base de datos local
- `GET /api/personajes/GetAll/?page=1&pageSize=20&nombre=Rick&estado=Alive` . Consulta todos los registros de personsajes bases de datos con paginador
- `GET /api/personajes/GetByID/{id}`. Consulta registro de personaje especifico 
- `POST /api/solicitudes/Crear`. Guardar Solicitud nueva
- `GET /api/solicitudes/GetAll/?estado=Pendiente&cliente=Juan`.  Consulta todos los registros de solicitudes guardados en bases de datos con paginador
- `GET /api/solicitudes/GetById/{id}`. Consulta solicitudes de personaje especifico 
- `PATCH /api/solicitudes/CambiarEstado/{id}`. Cambiar estado del evento solicitado
- `GET /api/reportes/Resumen`. Consulta de reportes solicitados
- `GET /health`. Consulta estado del API desde Azure.

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

## 8. Ejercicio de Migración — Web Forms a .NET 8

Problemas identificados en el codigo legado:

1. Credenciales hardcodeadas en el code-behind. (No estan encriptadas BPCript por ejemplo comp un algoritmo válido o colocado en un archivo json)
2. SQL concatenado, vulnerable a SQL Injection.
3. Logica de UI, validacion, acceso a datos y negocio mezcladas. (Sin separacion de estructura)
4. Sin manejo global de errores ni logging. 
5. Uso de `DateTime.Now` sin criterio UTC.
6. Estado guardado en `Session`, dificil de escalar en cloud.
7. Sin pruebas unitarias.

Ejemplo de Reescritura equivalente:

```csharp
    /// <summary>
    /// Guardar Solicitud
    /// </summary>
    /// <param name="request"></param>
    /// <returns> IActionResult </returns>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request, CancellationToken ct)
    {
        try
        {
            var created = await service.CrearAsync(request, ct);

            if (created.Id > 0)
            {
                return Ok(created);
            }
            else
            {
                return BadRequest(created);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
```

La validacion queda en FluentValidation, la persistencia en EF Core y los errores en middleware global.

## 9. Preguntas de Liderazgo Técnico

### ¿Como planificarías la migración completa del sistema legado en etapas graduales?

Primero haria una investigación y estudio del sistema legacy, inventario de pantallas, procesos criticos, datos y dependencias. Luego construiria APIs nuevas por bounded context, empezando por modulos de bajo riesgo. Usaria microservicios para reemplazar funciones gradualmente, manteniendo pruebas de regresion y monitoreo.

### ¿Qué estrategia usarías si el sistema legado debe operar en paralelo durante la transición?

Usaria convivencia temporal con sincronizacion controlada de datos y rutas por modulo y logs comparativos. Para evitar inconsistencias definiria una fuente de verdad por entidad durante la transicion.

### ¿Como organizarías a un equipo de 3 desarrolladores para este módulo? (roles, code reviews, ramas Git)

- Dev 1: API/Application y reglas de negocio,  Infrastructure, EF Core, Rick and Morty client y Docker (Persobajes).
- Dev 2: Frontend, API/Application y reglas de negocio,  Infrastructure (Solicitudes).
- Dev 3: pruebas, documentacion, CI/CD.

- Ante todo apoyarse como equipo antes posibles riesgos de entrega para llegar al objetivo de publicación del API.

Flujo Git: ramas cortas por feature, pull requests obligatorios, minimo una aprobación para envio a los otros ambientes, validacion de build/tests antes de merge a `main`.

## Pruebas

```bash
dotnet test tests/IntergalaxyTech.Tests/IntergalaxyTech.Tests.csproj
```

## Herramientas IA utilizadas

Se utilizó ChatGPT como apoyo para estructurar la solución, generar estructura inicial, README y recomendaciones de arquitectura. 
