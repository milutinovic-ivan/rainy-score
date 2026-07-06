#!/bin/bash

set -e

RETENTION_DAYS=60

cd /opt/rainyscore

source .env

mkdir -p backups

BACKUP_FILE="backups/rainy_$(date +%F_%H%M%S).sql.gz"

docker exec rainy_postgres pg_dump \
    -U "$POSTGRES_USER" \
    "$POSTGRES_DB" \
| gzip > "$BACKUP_FILE"

find backups -type f -name "*.sql.gz" -mtime +$RETENTION_DAYS -delete

echo
echo "✅ Backup created:"
echo "$BACKUP_FILE"