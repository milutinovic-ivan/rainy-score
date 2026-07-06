#!/bin/bash

set -e

cd /opt/rainyscore

source .env

echo "Available backups:"
ls -1 backups/*.sql.gz | xargs -n1 basename

echo
read -p "Backup file: " BACKUP

gunzip -c "backups/$BACKUP" | docker exec -i rainy_postgres psql \
    -U "$POSTGRES_USER" \
    "$POSTGRES_DB"

echo
echo "✅ Restore completed."