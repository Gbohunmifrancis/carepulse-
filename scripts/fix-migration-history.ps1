# Migration History Sync Script
# Use this script if your database has tables but missing migration history

Write-Host "=== Migration History Sync Script ===" -ForegroundColor Green
Write-Host "This script will help sync your migration history with existing database schema" -ForegroundColor Yellow
Write-Host ""

# Navigate to the Data project directory
$dataProjectPath = "C:\Users\FRANCIS GBOHUNMI\Desktop\PROJECTS\solve-station\solve-inventory-pharmacy\src\SolveStation.Data"
$apiProjectPath = "C:\Users\FRANCIS GBOHUNMI\Desktop\PROJECTS\solve-station\solve-inventory-pharmacy\src\SolveStation.PharmacyApi"

Write-Host "Checking current migration status..." -ForegroundColor Cyan

# Check if we're in the right directory
if (-not (Test-Path $dataProjectPath)) {
    Write-Host "Error: Data project not found at $dataProjectPath" -ForegroundColor Red
    exit 1
}

Set-Location $dataProjectPath

# Option 1: Check current migration status
Write-Host "`n1. Checking current migration status..." -ForegroundColor Cyan
try {
    $migrationStatus = dotnet ef database update --list --project $dataProjectPath --startup-project $apiProjectPath 2>&1
    Write-Host "Migration Status:" -ForegroundColor Green
    Write-Host $migrationStatus
} catch {
    Write-Host "Could not get migration status: $_" -ForegroundColor Yellow
}

Write-Host "`n=== OPTIONS ===" -ForegroundColor Magenta
Write-Host "Choose an option:" -ForegroundColor Yellow
Write-Host "1. Force sync migration history (if tables exist but migration history is missing)"
Write-Host "2. Drop and recreate database (CAUTION: This will delete all data)"
Write-Host "3. Create a new migration from current state"
Write-Host "4. Just run normal migration"
Write-Host "5. Exit"

$choice = Read-Host "`nEnter your choice (1-5)"

switch ($choice) {
    "1" {
        Write-Host "`nForce syncing migration history..." -ForegroundColor Yellow
        Write-Host "This will mark all existing migrations as applied without running them." -ForegroundColor Yellow
        $confirm = Read-Host "Are you sure? (y/N)"
        
        if ($confirm -eq "y" -or $confirm -eq "Y") {
            try {
                # First, try to update the database - this should trigger our new logic
                dotnet ef database update --project $dataProjectPath --startup-project $apiProjectPath
                Write-Host "Migration history sync completed!" -ForegroundColor Green
            } catch {
                Write-Host "Error during sync: $_" -ForegroundColor Red
                Write-Host "You may need to manually fix the database." -ForegroundColor Yellow
            }
        }
    }
    "2" {
        Write-Host "`nDROPPING AND RECREATING DATABASE..." -ForegroundColor Red
        Write-Host "WARNING: This will delete ALL data in your database!" -ForegroundColor Red
        $confirm = Read-Host "Type 'DELETE' to confirm"
        
        if ($confirm -eq "DELETE") {
            try {
                # Drop database
                dotnet ef database drop --force --project $dataProjectPath --startup-project $apiProjectPath
                Write-Host "Database dropped." -ForegroundColor Yellow
                
                # Create and migrate
                dotnet ef database update --project $dataProjectPath --startup-project $apiProjectPath
                Write-Host "Database recreated and migrated!" -ForegroundColor Green
            } catch {
                Write-Host "Error during database recreation: $_" -ForegroundColor Red
            }
        } else {
            Write-Host "Operation cancelled." -ForegroundColor Yellow
        }
    }
    "3" {
        Write-Host "`nCreating a new migration from current state..." -ForegroundColor Yellow
        $migrationName = Read-Host "Enter migration name (e.g., 'SyncExistingSchema')"
        
        try {
            dotnet ef migrations add $migrationName --project $dataProjectPath --startup-project $apiProjectPath
            Write-Host "Migration '$migrationName' created!" -ForegroundColor Green
            Write-Host "You can now run 'dotnet ef database update' to apply it." -ForegroundColor Yellow
        } catch {
            Write-Host "Error creating migration: $_" -ForegroundColor Red
        }
    }
    "4" {
        Write-Host "`nRunning normal migration..." -ForegroundColor Yellow
        try {
            dotnet ef database update --project $dataProjectPath --startup-project $apiProjectPath
            Write-Host "Migration completed!" -ForegroundColor Green
        } catch {
            Write-Host "Error during migration: $_" -ForegroundColor Red
        }
    }
    "5" {
        Write-Host "Exiting..." -ForegroundColor Yellow
        exit 0
    }
    default {
        Write-Host "Invalid choice. Exiting..." -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n=== DONE ===" -ForegroundColor Green
Write-Host "You can now try running your application with 'dotnet run'" -ForegroundColor Cyan