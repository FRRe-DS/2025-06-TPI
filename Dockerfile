# =========================================================
# STAGE 1: BUILD
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ApiDePapas.sln", "./"]
COPY src/ApiDePapas/ApiDePapas.csproj src/ApiDePapas/
COPY src/ApiDePapas.Domain/ApiDePapas.Domain.csproj src/ApiDePapas.Domain/
COPY src/ApiDePapas.Application/ApiDePapas.Application.csproj src/ApiDePapas.Application/
COPY src/ApiDePapas.Infrastructure/ApiDePapas.Infrastructure.csproj src/ApiDePapas.Infrastructure/
RUN dotnet restore "ApiDePapas.sln"

# Copy the rest of the code and publish the application
COPY . .
RUN dotnet publish "src/ApiDePapas/ApiDePapas.csproj" -c Release -o /app/publish

# =========================================================
# STAGE 2: FINAL
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Install tools (MySQL client and netcat)
RUN apt-get update && apt-get install -y mariadb-client netcat-openbsd && rm -rf /var/lib/apt/lists/*

# Install Entity Framework Core tools globally
RUN dotnet tool install --global dotnet-ef

# Add dotnet tools to the PATH so the entrypoint can find them
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy the published files from the 'build' stage
COPY --from=build /app/publish .

# Copy the project files (.csproj) and solution (.sln) to a subfolder.
# This gives 'dotnet ef' the files it needs to work.
COPY --from=build /src .

# Copy the entrypoint script and give it execute permissions
COPY entrypoint.sh .
RUN chmod +x ./entrypoint.sh

EXPOSE 8080
ENTRYPOINT ["/bin/bash", "./entrypoint.sh"]
