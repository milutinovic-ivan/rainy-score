<#
.SYNOPSIS
    Restores PostgreSQL database from backup

.PARAMETER BackupFile
    Path to the SQL backup file

.PARAMETER ContainerName
    Docker container name (default: rainy_postgres)

.PARAMETER DbName
    Database name (default: RainyScore)

.EXAMPLE
    .\restore-docker-db.ps1

.EXAMPLE
    .\restore-docker-db.ps1 -BackupFile ".\database_backups\backup_RainyScore_2025-01-15_10-30-45.sql"
#>

param(
    [string]$BackupFile,
    [string]$ContainerName = "rainy_postgres",
    [string]$DbName = "RainyScore",
    [string]$DbUser = "postgres",
    [string]$DbPassword = "1234"
)

# If no backup file specified, let user choose
if (-not $BackupFile) {
    $BackupDir = Join-Path $PSScriptRoot "database_backups"
    if (Test-Path $BackupDir) {
        $Backups = Get-ChildItem $BackupDir -Filter "*.sql" | Sort-Object -Property CreationTime -Descending
        
        if ($Backups.Count -eq 0) {
            Write-Host "[ERROR] No backup files found in: $BackupDir" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "Available backups:" -ForegroundColor Cyan
        for ($i = 0; $i -lt $Backups.Count; $i++) {
            $Size = $Backups[$i].Length / 1KB
            Write-Host "  [$($i+1)] $($Backups[$i].Name) ($([Math]::Round($Size, 0)) KB)"
        }
        
        $Selection = Read-Host "Select backup number (1-$($Backups.Count))"
        $BackupFile = $Backups[$Selection - 1].FullName
    }
    else {
        Write-Host "[ERROR] Backup directory not found: $BackupDir" -ForegroundColor Red
        exit 1
    }
}

# Validate backup file exists
if (-not (Test-Path $BackupFile)) {
    Write-Host "[ERROR] Backup file not found: $BackupFile" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Database Restore" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Restore Configuration:" -ForegroundColor Yellow
Write-Host "  Backup File:  $BackupFile"
Write-Host "  Container:    $ContainerName"
Write-Host "  Database:     $DbName"
Write-Host ""

$Confirm = Read-Host "Are you sure you want to restore? This will overwrite the database (yes/no)"
if ($Confirm -ne "yes") {
    Write-Host "Restore cancelled." -ForegroundColor Yellow
    exit 0
}

# Ensure container is running
Write-Host "Ensuring container is running..." -ForegroundColor Gray
$ContainerStatus = docker inspect -f '{{.State.Running}}' $ContainerName 2>$null
if ($ContainerStatus -ne "true") {
    Write-Host "Starting container..." -ForegroundColor Gray
    docker compose up -d postgres 2>$null
    Start-Sleep -Seconds 5
}

# Drop and recreate database
Write-Host "Preparing database..." -ForegroundColor Gray
Write-Host "  Dropping existing database..." -ForegroundColor Gray
docker exec -e PGPASSWORD=$DbPassword $ContainerName `
    psql -U $DbUser -c "DROP DATABASE IF EXISTS $DbName;" 2>$null

Write-Host "  Creating new database..." -ForegroundColor Gray
docker exec -e PGPASSWORD=$DbPassword $ContainerName `
    psql -U $DbUser -c "CREATE DATABASE $DbName;" 2>$null

# Restore from backup
Write-Host "Restoring from backup..." -ForegroundColor Gray
Get-Content $BackupFile | docker exec -i -e PGPASSWORD=$DbPassword $ContainerName `
    psql -U $DbUser -d $DbName 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  RESTORE COMPLETED SUCCESSFULLY" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    exit 0
}
else {
    Write-Host ""
    Write-Error-Custom "Restore failed! Please check the backup file and try again."
    exit 1
}