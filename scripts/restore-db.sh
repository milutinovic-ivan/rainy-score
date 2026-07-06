#!/bin/bash

set -e

cd /opt/rainyscore

source .env

echo "Available backups:"
ls -1 backups/*.dump | xargs -n1 basename

echo
read -p "Backup file: " BACKUP

echo
echo "⚠️  This will REPLACE all data in '$POSTGRES_DB'."
read -p "Continue? (yes/no): " ANSWER

if [ "$ANSWER" != "yes" ]; then
    echo "Restore cancelled."
    exit 0
fi

docker cp "backups/$BACKUP" rainy_postgres:/tmp/restore.dump

docker exec rainy_postgres pg_restore \
    -U "$POSTGRES_USER" \
    -d "$POSTGRES_DB" \
    --clean \
    --if-exists \
    --no-owner \
    --no-privileges \
    /tmp/restore.dump

docker exec rainy_postgres rm /tmp/restore.dump

echo
echo "✅ Restore completed."