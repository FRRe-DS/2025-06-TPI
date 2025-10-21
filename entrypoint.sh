#!/bin/bash
set -e # Falla si algo sale mal

DB_HOST="db"
DB_PORT="3306"
DB_USER="ApiUser"
DB_NAME="apidepapas"

echo "API -> Esperando que MySQL ($DB_HOST:$DB_PORT) inicie..."
until nc -z "$DB_HOST" "$DB_PORT"; do
  sleep 1
done
echo "MySQL iniciado."

# --- ¡LA LÓGICA CLAVE ESTÁ AQUÍ! ---
# Ejecutamos una consulta para ver si la tabla __EFMigrationsHistory ya existe.
# Esta tabla es creada por Entity Framework después de la primera migración exitosa.
MIGRATIONS_TABLE_EXISTS=$(mysql -h "$DB_HOST" -u "$DB_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" -sN -e "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '$DB_NAME' AND table_name = '__EFMigrationsHistory';")

if [ "$MIGRATIONS_TABLE_EXISTS" -eq 0 ]; then
  echo "La base de datos parece ser nueva. Aplicando migraciones y cargando datos..."

  # 1. Aplicar migraciones de EF Core
  cd /app/src/ApiDePapas.Infrastructure
  dotnet ef database update --startup-project ../ApiDePapas
  echo "Migraciones aplicadas."

  # 2. Cargar datos masivos
  cd /app
  if [ -f "/app/db-init/load_all_data.sql" ]; then
    mysql --local-infile=1 -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" < /app/db-init/load_all_data.sql
    echo "Carga masiva completada."
  else
    echo "No se encontró /app/db-init/load_all_data.sql, se omite la carga masiva."
  fi
else
  echo "La base de datos ya está inicializada. Omitiendo migraciones y carga de datos."
fi

echo "Iniciando la API..."
exec dotnet ApiDePapas.dll