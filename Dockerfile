# =========================================================
# ETAPA 1: BUILD
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y csproj (esto habilita cache del restore)
COPY ApiDePapas.sln ./
COPY src/ApiDePapas/*.csproj src/ApiDePapas/
COPY src/ApiDePapas.Domain/*.csproj src/ApiDePapas.Domain/
COPY src/ApiDePapas.Application/*.csproj src/ApiDePapas.Application/
COPY src/ApiDePapas.Infrastructure/*.csproj src/ApiDePapas.Infrastructure/

# Restaurar dependencias
RUN dotnet restore ApiDePapas.sln

# Copiar TODO el código
COPY . .

# Publicar
WORKDIR "/src/src/ApiDePapas"
RUN dotnet publish -c Release -o /app/publish

# =========================================================
# ETAPA 2: FINAL
# =========================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# utilidades
RUN apt-get update && apt-get install -y netcat-openbsd sed && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

COPY entrypoint.sh .
RUN chmod +x ./entrypoint.sh

ENTRYPOINT ["/bin/bash", "./entrypoint.sh"]
