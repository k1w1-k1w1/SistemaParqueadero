# Build context = raiz del repo (donde esta el .sln)
# Este archivo va en: SistemaParqueadero.API/Dockerfile

# ── Etapa 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SistemaParqueadero/Models.csproj", "SistemaParqueadero/"]
COPY ["SistemaParqueadero.API/SistemaParqueadero.API.csproj", "SistemaParqueadero.API/"]

RUN dotnet restore "SistemaParqueadero.API/SistemaParqueadero.API.csproj"

COPY ["SistemaParqueadero/", "SistemaParqueadero/"]
COPY ["SistemaParqueadero.API/", "SistemaParqueadero.API/"]

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