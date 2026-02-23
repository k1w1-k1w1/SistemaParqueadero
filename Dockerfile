	# ── Etapa 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar .csproj primero (mejor cache de capas)
COPY ["SistemaParqueadero/Models.csproj", "SistemaParqueadero/"]
COPY ["SistemaParqueadero.API/SistemaParqueadero.API.csproj", "SistemaParqueadero.API/"]

# Restaurar dependencias
RUN dotnet restore "SistemaParqueadero.API/SistemaParqueadero.API.csproj"

# Copiar todo el código fuente
COPY ["SistemaParqueadero/", "SistemaParqueadero/"]
COPY ["SistemaParqueadero.API/", "SistemaParqueadero.API/"]

# Publicar en Release
RUN dotnet publish "SistemaParqueadero.API/SistemaParqueadero.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Etapa 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

ENTRYPOINT ["dotnet", "SistemaParqueadero.API.dll"]