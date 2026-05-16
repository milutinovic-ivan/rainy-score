param(
    [string]$ContainerName = "rainy_postgres",
    [string]$DbName = "RainyScore",
    [string]$DbUser = "postgres",
    [string]$DbPassword = "1234",
    [string]$BackupDir = "$(Split-Path -Parent $PSScriptRoot)\database_backups"
)

# Create backup directory
New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null

# Generate backup filename with timestamp
$Timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$BackupFile = "$BackupDir\backup_$($DbName)_$Timestamp.sql"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Docker Database Backup Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backup Configuration:" -ForegroundColor Yellow
Write-Host "  Container:    $ContainerName"
Write-Host "  Database:     $DbName"
Write-Host "  Username:     $DbUser"
Write-Host "  Backup File:  $BackupFile"
Write-Host ""

# Check if container is running
Write-Host "Checking container status..." -ForegroundColor Gray
$ContainerStatus = docker inspect -f '{{.State.Running}}' $ContainerName 2>$null

if ($ContainerStatus -ne "true") {
    Write-Host "[WARNING] Container is not running. Starting it..." -ForegroundColor Yellow
    docker compose up -d postgres 2>$null
    Start-Sleep -Seconds 5
}

Write-Host "[OK] Container is running" -ForegroundColor Green
Write-Host ""

# Wait for database to be ready
Write-Host "Waiting for database to be ready..." -ForegroundColor Gray
$MaxRetries = 30
$Retry = 0
$DbReady = $false

while ($Retry -lt $MaxRetries -and -not $DbReady) {
    try {
        docker exec -e PGPASSWORD=$DbPassword $ContainerName `
            psql -U $DbUser -d $DbName -c "SELECT 1;" >$null 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            $DbReady = $true
            Write-Host "[OK] Database is ready" -ForegroundColor Green
        }
    }
    catch {
        # Database not ready yet
    }
    
    if (-not $DbReady) {
        $Retry++
        if ($Retry -lt $MaxRetries) {
            Write-Host "  Attempt $Retry/$MaxRetries..." -ForegroundColor Gray
            Start-Sleep -Seconds 1
        }
    }
}

if (-not $DbReady) {
    Write-Host "[ERROR] Database failed to become ready" -ForegroundColor Red
    exit 1
}

# Create backup
Write-Host ""
Write-Host "Creating backup..." -ForegroundColor Cyan

try {
    docker exec -e PGPASSWORD=$DbPassword $ContainerName `
        pg_dump -U $DbUser -d $DbName > $BackupFile 2>$null

    if ($LASTEXITCODE -ne 0) {
        throw "pg_dump command failed"
    }
}
catch {
    Write-Host "[ERROR] Backup failed: $_" -ForegroundColor Red
    if (Test-Path $BackupFile) {
        Remove-Item $BackupFile -Force
    }
    exit 1
}

# Verify backup
if ((Test-Path $BackupFile) -and ((Get-Item $BackupFile).Length -gt 0)) {
    $FileSize = (Get-Item $BackupFile).Length
    $FileSizeKB = $FileSize / 1KB
    $FileSizeMB = $FileSize / 1MB

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  BACKUP COMPLETED SUCCESSFULLY" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Backup Details:" -ForegroundColor Yellow
    Write-Host "  File:     $BackupFile"
    Write-Host "  Size:     $([Math]::Round($FileSizeMB, 2)) MB"
    Write-Host "  Created:  $(Get-Item $BackupFile | Select-Object -ExpandProperty CreationTime)"
    Write-Host ""
    Write-Host "[OK] Ready to run migrations!" -ForegroundColor Green
    Write-Host ""
    exit 0
}
else {
    Write-Host "[ERROR] Backup file is empty or was not created" -ForegroundColor Red
    if (Test-Path $BackupFile) {
        Remove-Item $BackupFile -Force
    }
    exit 1
}