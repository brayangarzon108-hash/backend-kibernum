FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/IntergalaxyTech.Domain/IntergalaxyTech.Domain.csproj src/IntergalaxyTech.Domain/
COPY src/IntergalaxyTech.Application/IntergalaxyTech.Application.csproj src/IntergalaxyTech.Application/
COPY src/IntergalaxyTech.Infrastructure/IntergalaxyTech.Infrastructure.csproj src/IntergalaxyTech.Infrastructure/
COPY src/IntergalaxyTech.Api/IntergalaxyTech.Api.csproj src/IntergalaxyTech.Api/
RUN dotnet restore src/IntergalaxyTech.Api/IntergalaxyTech.Api.csproj
COPY . .
RUN dotnet publish src/IntergalaxyTech.Api/IntergalaxyTech.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IntergalaxyTech.Api.dll"]
