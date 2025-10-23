#!/bin/bash
# IMPORTANT: This file must be saved with UTF-8 encoding and LF line endings.
set -e # Exit immediately if a command exits with a non-zero status.

DB_HOST="db"
DB_PORT="3306"
DB_USER="ApiUser"
DB_NAME="apidepapas"

echo "API -> Waiting for MySQL ($DB_HOST:$DB_PORT) to start..."
# Use netcat (nc) to check if the port is open.
until nc -z "$DB_HOST" "$DB_PORT"; do
  sleep 1
done
echo "MySQL started."

# Check if the database has already been initialized by looking for the EF migrations table.
MIGRATIONS_TABLE_EXISTS=$(mysql -h "$DB_HOST" -u "$DB_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" -sN -e "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '$DB_NAME' AND table_name = '__EFMigrationsHistory';")

if [ "$MIGRATIONS_TABLE_EXISTS" -eq 0 ]; then
  echo "Database appears to be new. Applying migrations and seeding data..."

  # 1. Apply EF Core migrations
  # We need to be in the directory of the infrastructure project to run the command.
  cd /app/src/ApiDePapas.Infrastructure
  dotnet ef database update --startup-project ../ApiDePapas
  echo "Migrations applied."

  # 2. Load bulk data from SQL script
  cd /app
  if [ -f "/app/db-init/load_all_data.sql" ]; then
    mysql --local-infile=1 -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$MYSQL_PASSWORD" "$DB_NAME" < /app/db-init/load_all_data.sql
    echo "Bulk data load completed."
  else
    echo "Could not find /app/db-init/load_all_data.sql, skipping bulk data load."
  fi
else
  echo "Database already initialized. Skipping migrations and data seeding."
fi

echo "Starting the API..."
# Execute the main process of the container
exec dotnet ApiDePapas.dll
