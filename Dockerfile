# =========================================================
# ETAPA 1: BUILD (Construcción)
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos de proyecto y restaura dependencias
COPY ["ApiDePapas.sln", "./"]
COPY src/ApiDePapas/ApiDePapas.csproj src/ApiDePapas/
COPY src/ApiDePapas.Domain/ApiDePapas.Domain.csproj src/ApiDePapas.Domain/
COPY src/ApiDePapas.Application/ApiDePapas.Application.csproj src/ApiDePapas.Application/
COPY src/ApiDePapas.Infrastructure/ApiDePapas.Infrastructure.csproj src/ApiDePapas.Infrastructure/
RUN dotnet restore "ApiDePapas.sln"

# Copia el resto del código y publica la aplicación
COPY . .
RUN dotnet publish "src/ApiDePapas/ApiDePapas.csproj" -c Release -o /app/publish

# =========================================================
# ETAPA 2: FINAL (Ejecución y Orquestación)
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Instala herramientas (cliente MySQL y netcat)
RUN apt-get update && apt-get install -y mariadb-client netcat-openbsd && rm -rf /var/lib/apt/lists/*

# 1. Desinstala cualquier versión previa o cacheada (el '|| true' ignora errores si no estaba instalado)
RUN dotnet tool uninstall --global dotnet-ef --verbosity quiet || true

# 2. Instala una versión específica y estable (coincidente con tu SDK 8.0)
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Agrega las herramientas de dotnet al PATH para que el entrypoint las encuentre
ENV PATH="$PATH:/root/.dotnet/tools"

# Copia los archivos publicados desde la etapa 'build'
COPY --from=build /app/publish .

# Copia los archivos de proyecto (.csproj) y la solución (.sln) a una subcarpeta.
# Esto le da a 'dotnet ef' los archivos que necesita para trabajar.
COPY --from=build /src .

# Copia el script de inicio y le da permisos
COPY entrypoint.sh .
RUN chmod +x ./entrypoint.sh

EXPOSE 8080
ENTRYPOINT ["/bin/bash", "./entrypoint.sh"]