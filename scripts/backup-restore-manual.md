# Database Backup & Restore

## Create a backup

```bash
cd /opt/rainyscore
./scripts/backup-db.sh
```

The backup is stored in:

```text
/opt/rainyscore/backups/
```

Example:

```text
rainy_2026-07-06_105410.dump
```

Backups older than **60 days** are automatically deleted.

---

## Restore a backup

> ⚠️ Restoring replaces all current database data.

(Optional but recommended) Create a fresh backup first:

```bash
./scripts/backup-db.sh
```

Run the restore:

```bash
./scripts/restore-db.sh
```

The script will:

1. Show all available backups.
2. Ask which backup to restore.
3. Ask for confirmation.
4. Restore the selected backup.

---

## Make scripts executable (only once)

```bash
chmod +x scripts/backup-db.sh
chmod +x scripts/restore-db.sh
```