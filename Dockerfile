# =========================================================
# ETAPA 1: BUILD (Construcción)
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia y restaura dependencias
COPY ["ApiDePapas.sln", "./"]
COPY src/ApiDePapas/ApiDePapas.csproj src/ApiDePapas/
COPY src/ApiDePapas.Domain/ApiDePapas.Domain.csproj src/ApiDePapas.Domain/
COPY src/ApiDePapas.Application/ApiDePapas.Application.csproj src/ApiDePapas.Application/
COPY src/ApiDePapas.Infrastructure/ApiDePapas.Infrastructure.csproj src/ApiDePapas.Infrastructure/
RUN dotnet restore "ApiDePapas.sln"

# Copia el resto del código y publica
COPY . .
WORKDIR "/src/src/ApiDePapas"
RUN dotnet publish "ApiDePapas.csproj" -c Release -o /app/publish

# =========================================================
# ETAPA 2: FINAL (Ejecución)
# ¡Usamos la imagen ASPNET liviana!
# =========================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# --- ¡CAMBIO AQUÍ! ---
# Instalamos netcat (para esperar) Y sed (para limpiar CSVs)
RUN apt-get update && apt-get install -y netcat-openbsd sed && \
    rm -rf /var/lib/apt/lists/*

<<<<<<< HEAD
# Copia SÓLO la aplicación compilada
=======
# 1. Desinstala cualquier versión previa o cacheada (el '|| true' ignora errores si no estaba instalado)
RUN dotnet tool uninstall --global dotnet-ef --verbosity quiet || true

# 2. Instala una versión específica y estable (coincidente con tu SDK 8.0)
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Agrega las herramientas de dotnet al PATH para que el entrypoint las encuentre
ENV PATH="$PATH:/root/.dotnet/tools"

# Copia los archivos publicados desde la etapa 'build'
>>>>>>> origin/Features/endpoints
COPY --from=build /app/publish .

# --- ¡LIMPIEZA! ---
# Borramos los comentarios viejos sobre 'db-init' porque ya no se usa.

# Copia el script de inicio (el NUEVO, simple)
COPY entrypoint.sh .
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["/bin/bash", "./entrypoint.sh"]