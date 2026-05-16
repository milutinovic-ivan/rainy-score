# Step 1: Backup the database
cd C:\gitroot\RainyScore
.\backup-docker-db.ps1

# Wait for backup to complete and verify success
# Output will show: "✓ Backup completed successfully!"


# Check backups folder
Get-ChildItem .\database_backups\*.sql | Sort-Object CreationTime -Descending | Select-Object -First 3

# Optional step: Run migration Using dotnet CLI
dotnet ef database update

# Restore from the backup you just created
.\restore-docker-db.ps1

# It will show you available backups to choose from