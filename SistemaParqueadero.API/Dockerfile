# ====== BUILD STAGE ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y csproj primero (mejor caché)
COPY *.sln ./

# Ajusta estos paths según tu repo:
COPY SistemaParqueadero.Models/SistemaParqueadero.Models.csproj SistemaParqueadero.Models/
COPY SistemaParqueadero.API/SistemaParqueadero.API.csproj SistemaParqueadero.API/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . .

# Publicar
RUN dotnet publish SistemaParqueadero.API/SistemaParqueadero.API.csproj -c Release -o /app/publish

# ====== RUNTIME STAGE ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Render usa PORT. ASP.NET escucha en 0.0.0.0
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Copiar publish
COPY --from=build /app/publish .

# Exponer puerto (Render lo maneja igual, pero ayuda)
EXPOSE 10000

# Iniciar
ENTRYPOINT ["dotnet", "SistemaParqueadero.API.dll"]