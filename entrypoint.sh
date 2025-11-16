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

echo "¡Base de datos lista!"

<<<<<<< HEAD
# --- ¡NUEVO! ---
# Limpiamos los saltos de línea de Windows (\r) de TODOS los archivos CSV
# antes de que la aplicación C# intente leerlos.
echo "Limpiando saltos de línea de Windows de los archivos CSV..."
for file in /app/csvs/*.csv; do
  # Comprobamos si el archivo existe antes de intentar limpiarlo
  if [ -f "$file" ]; then
    sed -i 's/\r$//' "$file"
=======
if [ "$MIGRATIONS_TABLE_EXISTS" -eq 0 ]; then
  echo "La base de datos parece ser nueva. Aplicando migraciones y cargando datos..."

  # 1. Aplicar migraciones de EF Core
  cd /app/src/ApiDePapas.Infrastructure
  dotnet ef database update --startup-project ../ApiDePapas
  echo "Migraciones aplicadas."
  sed -i 's/\r$//' /app/csvs/_addresses.csv
  # 2. Cargar datos masivos
  cd /app
  if [ -f "/app/db-init/load_all_data.sql" ]; then
    mysql --local-infile=1 -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" < /app/db-init/load_all_data.sql
    echo "Carga masiva completada."
  else
    echo "No se encontró /app/db-init/load_all_data.sql, se omite la carga masiva."
>>>>>>> origin/Features/endpoints
  fi
done
echo "Limpieza de CSVs completada."
# --- FIN DEL BLOQUE NUEVO ---

echo "Iniciando la aplicación..."

# Ejecuta la aplicación de .NET
exec dotnet ApiDePapas.dll