#!/bin/bash
set -e # Falla si algo sale mal

DB_HOST="db"  # Asumiendo que tu servicio de DB se llama 'db' en docker-compose
DB_PORT="3306"

echo "API -> Esperando que la base de datos ($DB_HOST:$DB_PORT) esté disponible..."

# Bucle que usa netcat (nc) para verificar si el puerto está abierto
until nc -z "$DB_HOST" "$DB_PORT"; do
  echo "La base de datos no está lista, reintentando en 1 segundo..."
  sleep 1
done

echo "¡Base de datos lista! Iniciando la aplicación..."

# Ejecuta la aplicación de .NET
# (Reemplaza 'ApiDePapas.dll' si tu DLL principal es otra)
exec dotnet ApiDePapas.dll